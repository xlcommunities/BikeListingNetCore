using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BikeListing.Bikes
{
    public interface IBikeRepository : IRepository<Bike, Guid>
    {
        Task<BikeWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<BikeWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string model = null,
            int? frameSizeMin = null,
            int? frameSizeMax = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            Guid? manufacturerId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Bike>> GetListAsync(
                    string filterText = null,
                    string model = null,
                    int? frameSizeMin = null,
                    int? frameSizeMax = null,
                    decimal? priceMin = null,
                    decimal? priceMax = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            string model = null,
            int? frameSizeMin = null,
            int? frameSizeMax = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            Guid? manufacturerId = null,
            CancellationToken cancellationToken = default);
    }
}