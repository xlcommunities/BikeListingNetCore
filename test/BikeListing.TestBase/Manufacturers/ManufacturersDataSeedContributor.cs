using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using BikeListing.Manufacturers;

namespace BikeListing.Manufacturers
{
    public class ManufacturersDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ManufacturersDataSeedContributor(IManufacturerRepository manufacturerRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _manufacturerRepository = manufacturerRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _manufacturerRepository.InsertAsync(new Manufacturer
            (
                id: Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a"),
                name: "48adc9b313b5482d80a72c2c520343c573068ac7ceb4489689b60764b03e02b6df36c7eed2ff42979603c6e9df97bd7f9e95"
            ));

            await _manufacturerRepository.InsertAsync(new Manufacturer
            (
                id: Guid.Parse("6a5b2d68-e84c-4ef1-920f-7ad7066e9198"),
                name: "3e1708543efe42078a393256a0a477f15256d60abebc4d62a8bc45858be415e688d92ebf72f94c8fa1c4911a46703e05d611"
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}