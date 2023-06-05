using BikeListing.Shared;
using BikeListing.Manufacturers;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using BikeListing.Permissions;
using BikeListing.Bikes;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using BikeListing.Shared;

namespace BikeListing.Bikes
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BikeListingPermissions.Bikes.Default)]
    public class BikesAppService : ApplicationService, IBikesAppService
    {
        private readonly IDistributedCache<BikeExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IBikeRepository _bikeRepository;
        private readonly BikeManager _bikeManager;
        private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;

        public BikesAppService(IBikeRepository bikeRepository, BikeManager bikeManager, IDistributedCache<BikeExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Manufacturer, Guid> manufacturerRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _bikeRepository = bikeRepository;
            _bikeManager = bikeManager; _manufacturerRepository = manufacturerRepository;
        }

        public virtual async Task<PagedResultDto<BikeWithNavigationPropertiesDto>> GetListAsync(GetBikesInput input)
        {
            var totalCount = await _bikeRepository.GetCountAsync(input.FilterText, input.Model, input.FrameSizeMin, input.FrameSizeMax, input.PriceMin, input.PriceMax, input.ManufacturerId);
            var items = await _bikeRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Model, input.FrameSizeMin, input.FrameSizeMax, input.PriceMin, input.PriceMax, input.ManufacturerId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BikeWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BikeWithNavigationProperties>, List<BikeWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<BikeWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<BikeWithNavigationProperties, BikeWithNavigationPropertiesDto>
                (await _bikeRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<BikeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Bike, BikeDto>(await _bikeRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetManufacturerLookupAsync(LookupRequestDto input)
        {
            var query = (await _manufacturerRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Manufacturer>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Manufacturer>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(BikeListingPermissions.Bikes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _bikeRepository.DeleteAsync(id);
        }

        [Authorize(BikeListingPermissions.Bikes.Create)]
        public virtual async Task<BikeDto> CreateAsync(BikeCreateDto input)
        {
            if (input.ManufacturerId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Manufacturer"]]);
            }

            var bike = await _bikeManager.CreateAsync(
            input.ManufacturerId, input.Model, input.FrameSize, input.Price
            );

            return ObjectMapper.Map<Bike, BikeDto>(bike);
        }

        [Authorize(BikeListingPermissions.Bikes.Edit)]
        public virtual async Task<BikeDto> UpdateAsync(Guid id, BikeUpdateDto input)
        {
            if (input.ManufacturerId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Manufacturer"]]);
            }

            var bike = await _bikeManager.UpdateAsync(
            id,
            input.ManufacturerId, input.Model, input.FrameSize, input.Price, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Bike, BikeDto>(bike);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BikeExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var bikes = await _bikeRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Model, input.FrameSizeMin, input.FrameSizeMax, input.PriceMin, input.PriceMax);
            var items = bikes.Select(item => new
            {
                Model = item.Bike.Model,
                FrameSize = item.Bike.FrameSize,
                Price = item.Bike.Price,

                Manufacturer = item.Manufacturer?.Name,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Bikes.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new BikeExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}