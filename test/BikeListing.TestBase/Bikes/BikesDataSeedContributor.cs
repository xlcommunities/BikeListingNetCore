using BikeListing.Manufacturers;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using BikeListing.Bikes;

namespace BikeListing.Bikes
{
    public class BikesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IBikeRepository _bikeRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ManufacturersDataSeedContributor _manufacturersDataSeedContributor;

        public BikesDataSeedContributor(IBikeRepository bikeRepository, IUnitOfWorkManager unitOfWorkManager, ManufacturersDataSeedContributor manufacturersDataSeedContributor)
        {
            _bikeRepository = bikeRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _manufacturersDataSeedContributor = manufacturersDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _manufacturersDataSeedContributor.SeedAsync(context);

            await _bikeRepository.InsertAsync(new Bike
            (
                id: Guid.Parse("173f5a1b-e88d-4230-87dc-8c714fa2265b"),
                model: "94cbf64fcf56409db8c920b50d986475888c49af91a74c99b5c478609ec2a4d1c6a04f020ad5466c850f6e3f8626280439d8",
                frameSize: 1427663761,
                price: 28577741,
                manufacturerId: Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a")
            ));

            await _bikeRepository.InsertAsync(new Bike
            (
                id: Guid.Parse("5437369c-faa8-4e66-94a6-e04e7b7ee9c4"),
                model: "2b1bf40666724a879b88f8ac2efb834538f5b23340b54a1fa8c0d11625023fccc1d1fe7c8c1a45acb6fba2e0497385d298e9",
                frameSize: 1086492616,
                price: 691557815,
                manufacturerId: Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a")
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}