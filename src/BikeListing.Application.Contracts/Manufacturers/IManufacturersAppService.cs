using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BikeListing.Shared;

namespace BikeListing.Manufacturers
{
    public interface IManufacturersAppService : IApplicationService
    {
        Task<PagedResultDto<ManufacturerDto>> GetListAsync(GetManufacturersInput input);

        Task<ManufacturerDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ManufacturerDto> CreateAsync(ManufacturerCreateDto input);

        Task<ManufacturerDto> UpdateAsync(Guid id, ManufacturerUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ManufacturerExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}