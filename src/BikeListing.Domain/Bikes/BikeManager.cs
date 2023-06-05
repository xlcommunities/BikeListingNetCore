using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace BikeListing.Bikes
{
    public class BikeManager : DomainService
    {
        private readonly IBikeRepository _bikeRepository;

        public BikeManager(IBikeRepository bikeRepository)
        {
            _bikeRepository = bikeRepository;
        }

        public async Task<Bike> CreateAsync(
        Guid manufacturerId, string model, int frameSize, decimal price)
        {
            Check.NotNull(manufacturerId, nameof(manufacturerId));
            Check.NotNullOrWhiteSpace(model, nameof(model));
            Check.Length(model, nameof(model), BikeConsts.ModelMaxLength, BikeConsts.ModelMinLength);

            var bike = new Bike(
             GuidGenerator.Create(),
             manufacturerId, model, frameSize, price
             );

            return await _bikeRepository.InsertAsync(bike);
        }

        public async Task<Bike> UpdateAsync(
            Guid id,
            Guid manufacturerId, string model, int frameSize, decimal price, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(manufacturerId, nameof(manufacturerId));
            Check.NotNullOrWhiteSpace(model, nameof(model));
            Check.Length(model, nameof(model), BikeConsts.ModelMaxLength, BikeConsts.ModelMinLength);

            var bike = await _bikeRepository.GetAsync(id);

            bike.ManufacturerId = manufacturerId;
            bike.Model = model;
            bike.FrameSize = frameSize;
            bike.Price = price;

            bike.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _bikeRepository.UpdateAsync(bike);
        }

    }
}