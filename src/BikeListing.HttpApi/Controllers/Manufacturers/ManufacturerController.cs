using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using BikeListing.Manufacturers;
using Volo.Abp.Content;
using BikeListing.Shared;

namespace BikeListing.Controllers.Manufacturers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Manufacturer")]
    [Route("api/app/manufacturers")]

    public class ManufacturerController : AbpController, IManufacturersAppService
    {
        private readonly IManufacturersAppService _manufacturersAppService;

        public ManufacturerController(IManufacturersAppService manufacturersAppService)
        {
            _manufacturersAppService = manufacturersAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ManufacturerDto>> GetListAsync(GetManufacturersInput input)
        {
            return _manufacturersAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ManufacturerDto> GetAsync(Guid id)
        {
            return _manufacturersAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ManufacturerDto> CreateAsync(ManufacturerCreateDto input)
        {
            return _manufacturersAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ManufacturerDto> UpdateAsync(Guid id, ManufacturerUpdateDto input)
        {
            return _manufacturersAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _manufacturersAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ManufacturerExcelDownloadDto input)
        {
            return _manufacturersAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _manufacturersAppService.GetDownloadTokenAsync();
        }
    }
}