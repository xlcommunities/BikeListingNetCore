using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using BikeListing.Manufacturers;
using BikeListing.EntityFrameworkCore;
using Xunit;

namespace BikeListing.Manufacturers
{
    public class ManufacturerRepositoryTests : BikeListingEntityFrameworkCoreTestBase
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public ManufacturerRepositoryTests()
        {
            _manufacturerRepository = GetRequiredService<IManufacturerRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _manufacturerRepository.GetListAsync(
                    name: "48adc9b313b5482d80a72c2c520343c573068ac7ceb4489689b60764b03e02b6df36c7eed2ff42979603c6e9df97bd7f9e95"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _manufacturerRepository.GetCountAsync(
                    name: "3e1708543efe42078a393256a0a477f15256d60abebc4d62a8bc45858be415e688d92ebf72f94c8fa1c4911a46703e05d611"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}