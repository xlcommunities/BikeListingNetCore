using BikeListing.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using BikeListing.Bikes;
using Volo.Abp.Content;
using BikeListing.Shared;

namespace BikeListing.Controllers.Bikes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Bike")]
    [Route("api/app/bikes")]

    public class BikeController : AbpController, IBikesAppService
    {
        private readonly IBikesAppService _bikesAppService;

        public BikeController(IBikesAppService bikesAppService)
        {
            _bikesAppService = bikesAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<BikeWithNavigationPropertiesDto>> GetListAsync(GetBikesInput input)
        {
            return _bikesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<BikeWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _bikesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<BikeDto> GetAsync(Guid id)
        {
            return _bikesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("manufacturer-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetManufacturerLookupAsync(LookupRequestDto input)
        {
            return _bikesAppService.GetManufacturerLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<BikeDto> CreateAsync(BikeCreateDto input)
        {
            return _bikesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<BikeDto> UpdateAsync(Guid id, BikeUpdateDto input)
        {
            return _bikesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _bikesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(BikeExcelDownloadDto input)
        {
            return _bikesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _bikesAppService.GetDownloadTokenAsync();
        }
    }
}