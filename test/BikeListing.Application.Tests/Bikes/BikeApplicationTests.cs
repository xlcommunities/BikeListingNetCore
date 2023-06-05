using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace BikeListing.Bikes
{
    public class BikesAppServiceTests : BikeListingApplicationTestBase
    {
        private readonly IBikesAppService _bikesAppService;
        private readonly IRepository<Bike, Guid> _bikeRepository;

        public BikesAppServiceTests()
        {
            _bikesAppService = GetRequiredService<IBikesAppService>();
            _bikeRepository = GetRequiredService<IRepository<Bike, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _bikesAppService.GetListAsync(new GetBikesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Bike.Id == Guid.Parse("173f5a1b-e88d-4230-87dc-8c714fa2265b")).ShouldBe(true);
            result.Items.Any(x => x.Bike.Id == Guid.Parse("5437369c-faa8-4e66-94a6-e04e7b7ee9c4")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _bikesAppService.GetAsync(Guid.Parse("173f5a1b-e88d-4230-87dc-8c714fa2265b"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("173f5a1b-e88d-4230-87dc-8c714fa2265b"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new BikeCreateDto
            {
                Model = "719d7c7eac034d2fad4ac551f5cbf2dd49d81b6d79d44d9591815dcf5fc4e14661c1045d202349229dc06344c164d53de194",
                FrameSize = 1531238067,
                Price = 1143298606,
                ManufacturerId = Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a")
            };

            // Act
            var serviceResult = await _bikesAppService.CreateAsync(input);

            // Assert
            var result = await _bikeRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Model.ShouldBe("719d7c7eac034d2fad4ac551f5cbf2dd49d81b6d79d44d9591815dcf5fc4e14661c1045d202349229dc06344c164d53de194");
            result.FrameSize.ShouldBe(1531238067);
            result.Price.ShouldBe(1143298606);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new BikeUpdateDto()
            {
                Model = "7c9dea240d09417595a2083f0bf160685dc3aecd44b54dbe8c6eb2c6c72084efd704f695df4c4a72a471e8d1038a491780c7",
                FrameSize = 1398818377,
                Price = 1235669881,
                ManufacturerId = Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a")
            };

            // Act
            var serviceResult = await _bikesAppService.UpdateAsync(Guid.Parse("173f5a1b-e88d-4230-87dc-8c714fa2265b"), input);

            // Assert
            var result = await _bikeRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Model.ShouldBe("7c9dea240d09417595a2083f0bf160685dc3aecd44b54dbe8c6eb2c6c72084efd704f695df4c4a72a471e8d1038a491780c7");
            result.FrameSize.ShouldBe(1398818377);
            result.Price.ShouldBe(1235669881);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _bikesAppService.DeleteAsync(Guid.Parse("173f5a1b-e88d-4230-87dc-8c714fa2265b"));

            // Assert
            var result = await _bikeRepository.FindAsync(c => c.Id == Guid.Parse("173f5a1b-e88d-4230-87dc-8c714fa2265b"));

            result.ShouldBeNull();
        }
    }
}