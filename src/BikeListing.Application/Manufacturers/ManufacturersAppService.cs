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
using BikeListing.Manufacturers;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using BikeListing.Shared;

namespace BikeListing.Manufacturers
{
    [RemoteService(IsEnabled = false)]
    [Authorize(BikeListingPermissions.Manufacturers.Default)]
    public class ManufacturersAppService : ApplicationService, IManufacturersAppService
    {
        private readonly IDistributedCache<ManufacturerExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ManufacturerManager _manufacturerManager;

        public ManufacturersAppService(IManufacturerRepository manufacturerRepository, ManufacturerManager manufacturerManager, IDistributedCache<ManufacturerExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _manufacturerRepository = manufacturerRepository;
            _manufacturerManager = manufacturerManager;
        }

        public virtual async Task<PagedResultDto<ManufacturerDto>> GetListAsync(GetManufacturersInput input)
        {
            var totalCount = await _manufacturerRepository.GetCountAsync(input.FilterText, input.Name);
            var items = await _manufacturerRepository.GetListAsync(input.FilterText, input.Name, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ManufacturerDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(items)
            };
        }

        public virtual async Task<ManufacturerDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Manufacturer, ManufacturerDto>(await _manufacturerRepository.GetAsync(id));
        }

        [Authorize(BikeListingPermissions.Manufacturers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _manufacturerRepository.DeleteAsync(id);
        }

        [Authorize(BikeListingPermissions.Manufacturers.Create)]
        public virtual async Task<ManufacturerDto> CreateAsync(ManufacturerCreateDto input)
        {

            var manufacturer = await _manufacturerManager.CreateAsync(
            input.Name
            );

            return ObjectMapper.Map<Manufacturer, ManufacturerDto>(manufacturer);
        }

        [Authorize(BikeListingPermissions.Manufacturers.Edit)]
        public virtual async Task<ManufacturerDto> UpdateAsync(Guid id, ManufacturerUpdateDto input)
        {

            var manufacturer = await _manufacturerManager.UpdateAsync(
            id,
            input.Name, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Manufacturer, ManufacturerDto>(manufacturer);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ManufacturerExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _manufacturerRepository.GetListAsync(input.FilterText, input.Name);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Manufacturer>, List<ManufacturerExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Manufacturers.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ManufacturerExcelDownloadTokenCacheItem { Token = token },
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