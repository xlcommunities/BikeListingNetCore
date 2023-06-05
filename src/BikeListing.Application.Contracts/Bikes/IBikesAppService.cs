using BikeListing.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using BikeListing.Shared;

namespace BikeListing.Bikes
{
    public interface IBikesAppService : IApplicationService
    {
        Task<PagedResultDto<BikeWithNavigationPropertiesDto>> GetListAsync(GetBikesInput input);

        Task<BikeWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<BikeDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetManufacturerLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<BikeDto> CreateAsync(BikeCreateDto input);

        Task<BikeDto> UpdateAsync(Guid id, BikeUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(BikeExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}