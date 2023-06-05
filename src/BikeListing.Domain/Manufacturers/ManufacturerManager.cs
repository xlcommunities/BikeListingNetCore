using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace BikeListing.Manufacturers
{
    public class ManufacturerManager : DomainService
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public ManufacturerManager(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Manufacturer> CreateAsync(
        string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.Length(name, nameof(name), ManufacturerConsts.NameMaxLength, ManufacturerConsts.NameMinLength);

            var manufacturer = new Manufacturer(
             GuidGenerator.Create(),
             name
             );

            return await _manufacturerRepository.InsertAsync(manufacturer);
        }

        public async Task<Manufacturer> UpdateAsync(
            Guid id,
            string name, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.Length(name, nameof(name), ManufacturerConsts.NameMaxLength, ManufacturerConsts.NameMinLength);

            var manufacturer = await _manufacturerRepository.GetAsync(id);

            manufacturer.Name = name;

            manufacturer.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _manufacturerRepository.UpdateAsync(manufacturer);
        }

    }
}