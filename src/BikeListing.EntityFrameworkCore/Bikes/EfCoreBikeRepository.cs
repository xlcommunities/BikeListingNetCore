using BikeListing.Manufacturers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using BikeListing.EntityFrameworkCore;

namespace BikeListing.Bikes
{
    public class EfCoreBikeRepository : EfCoreRepository<BikeListingDbContext, Bike, Guid>, IBikeRepository
    {
        public EfCoreBikeRepository(IDbContextProvider<BikeListingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<BikeWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(bike => new BikeWithNavigationProperties
                {
                    Bike = bike,
                    Manufacturer = dbContext.Set<Manufacturer>().FirstOrDefault(c => c.Id == bike.ManufacturerId)
                }).FirstOrDefault();
        }

        public async Task<List<BikeWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, model, frameSizeMin, frameSizeMax, priceMin, priceMax, manufacturerId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BikeConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<BikeWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from bike in (await GetDbSetAsync())
                   join manufacturer in (await GetDbContextAsync()).Set<Manufacturer>() on bike.ManufacturerId equals manufacturer.Id into manufacturers
                   from manufacturer in manufacturers.DefaultIfEmpty()
                   select new BikeWithNavigationProperties
                   {
                       Bike = bike,
                       Manufacturer = manufacturer
                   };
        }

        protected virtual IQueryable<BikeWithNavigationProperties> ApplyFilter(
            IQueryable<BikeWithNavigationProperties> query,
            string filterText,
            string model = null,
            int? frameSizeMin = null,
            int? frameSizeMax = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            Guid? manufacturerId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Bike.Model.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(model), e => e.Bike.Model.Contains(model))
                    .WhereIf(frameSizeMin.HasValue, e => e.Bike.FrameSize >= frameSizeMin.Value)
                    .WhereIf(frameSizeMax.HasValue, e => e.Bike.FrameSize <= frameSizeMax.Value)
                    .WhereIf(priceMin.HasValue, e => e.Bike.Price >= priceMin.Value)
                    .WhereIf(priceMax.HasValue, e => e.Bike.Price <= priceMax.Value)
                    .WhereIf(manufacturerId != null && manufacturerId != Guid.Empty, e => e.Manufacturer != null && e.Manufacturer.Id == manufacturerId);
        }

        public async Task<List<Bike>> GetListAsync(
            string filterText = null,
            string model = null,
            int? frameSizeMin = null,
            int? frameSizeMax = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, model, frameSizeMin, frameSizeMax, priceMin, priceMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BikeConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string model = null,
            int? frameSizeMin = null,
            int? frameSizeMax = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            Guid? manufacturerId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, model, frameSizeMin, frameSizeMax, priceMin, priceMax, manufacturerId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Bike> ApplyFilter(
            IQueryable<Bike> query,
            string filterText,
            string model = null,
            int? frameSizeMin = null,
            int? frameSizeMax = null,
            decimal? priceMin = null,
            decimal? priceMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Model.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(model), e => e.Model.Contains(model))
                    .WhereIf(frameSizeMin.HasValue, e => e.FrameSize >= frameSizeMin.Value)
                    .WhereIf(frameSizeMax.HasValue, e => e.FrameSize <= frameSizeMax.Value)
                    .WhereIf(priceMin.HasValue, e => e.Price >= priceMin.Value)
                    .WhereIf(priceMax.HasValue, e => e.Price <= priceMax.Value);
        }
    }
}