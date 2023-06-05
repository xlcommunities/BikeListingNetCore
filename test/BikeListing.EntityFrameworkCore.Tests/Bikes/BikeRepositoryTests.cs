using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using BikeListing.Bikes;
using BikeListing.EntityFrameworkCore;
using Xunit;

namespace BikeListing.Bikes
{
    public class BikeRepositoryTests : BikeListingEntityFrameworkCoreTestBase
    {
        private readonly IBikeRepository _bikeRepository;

        public BikeRepositoryTests()
        {
            _bikeRepository = GetRequiredService<IBikeRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _bikeRepository.GetListAsync(
                    model: "94cbf64fcf56409db8c920b50d986475888c49af91a74c99b5c478609ec2a4d1c6a04f020ad5466c850f6e3f8626280439d8"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("173f5a1b-e88d-4230-87dc-8c714fa2265b"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _bikeRepository.GetCountAsync(
                    model: "2b1bf40666724a879b88f8ac2efb834538f5b23340b54a1fa8c0d11625023fccc1d1fe7c8c1a45acb6fba2e0497385d298e9"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}