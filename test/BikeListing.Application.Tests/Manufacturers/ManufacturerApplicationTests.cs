using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace BikeListing.Manufacturers
{
    public class ManufacturersAppServiceTests : BikeListingApplicationTestBase
    {
        private readonly IManufacturersAppService _manufacturersAppService;
        private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;

        public ManufacturersAppServiceTests()
        {
            _manufacturersAppService = GetRequiredService<IManufacturersAppService>();
            _manufacturerRepository = GetRequiredService<IRepository<Manufacturer, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _manufacturersAppService.GetListAsync(new GetManufacturersInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("6a5b2d68-e84c-4ef1-920f-7ad7066e9198")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _manufacturersAppService.GetAsync(Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ManufacturerCreateDto
            {
                Name = "de618e176cc247b49d59b92032621e30c03e8267fcd94aec829c1e3f371e1e203d533f1a0fe54e69aff2e3a3c018f73770c6"
            };

            // Act
            var serviceResult = await _manufacturersAppService.CreateAsync(input);

            // Assert
            var result = await _manufacturerRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("de618e176cc247b49d59b92032621e30c03e8267fcd94aec829c1e3f371e1e203d533f1a0fe54e69aff2e3a3c018f73770c6");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ManufacturerUpdateDto()
            {
                Name = "b7c29b20c59e47bbb3600829362d8dbe6541145802e04cd184ba336cafd65122ede7414c67bb4d068d0339b7c19c984fb7ac"
            };

            // Act
            var serviceResult = await _manufacturersAppService.UpdateAsync(Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a"), input);

            // Assert
            var result = await _manufacturerRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("b7c29b20c59e47bbb3600829362d8dbe6541145802e04cd184ba336cafd65122ede7414c67bb4d068d0339b7c19c984fb7ac");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _manufacturersAppService.DeleteAsync(Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a"));

            // Assert
            var result = await _manufacturerRepository.FindAsync(c => c.Id == Guid.Parse("3eccee6d-a290-455b-b4f5-62bb618e809a"));

            result.ShouldBeNull();
        }
    }
}