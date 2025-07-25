using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class RemoveCityAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;
        private readonly Mock<ILogger<Manager.CountryManager>> _logger;
        private readonly Mock<Manager.ICityManager> _cityManagerMock;

        public RemoveCityAsyncShould()
        {
            _countryRepository = new Mock<ICountryRepository>();
            _logger = new Mock<ILogger<Manager.CountryManager>>();

            _cityManagerMock = new Mock<Manager.ICityManager>();

            countryManager = new Manager.CountryManager(
                _logger.Object,
                _countryRepository.Object,
                _cityManagerMock.Object
            );
        }

        [Fact]
        public async Task Returns_failure_when_country_not_found()
        {
            int countryId = 1;
            var country = new Country { Id = countryId };
            var city = new City { Name = "Rio de Janeiro", Country = country, Province = "RJ" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.RemoveCityAsync(countryId, city, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CountryNotFound");

            _cityManagerMock.Verify(m => m.RemoveCityAsync(It.IsAny<int>(), It.IsAny<City>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Removes_city_successfully_when_country_exists()
        {
            int countryId = 2;
            var country = new Country { Id = countryId, Name = "Brazil", Code = "BRL" };
            var city = new City { Name = "Salvador", Country = country, Province = "BH" };
            var removedCity = new City { Id = 101, Name = "Salvador", Country = country, Province = "BH" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            _cityManagerMock
                .Setup(m => m.RemoveCityAsync(countryId, It.IsAny<City>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<City?>.Success(removedCity));

            var result = await countryManager.RemoveCityAsync(countryId, city, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("Salvador");
            result.Value.Country.Should().Be(country);

            _cityManagerMock.Verify(m => m.RemoveCityAsync(countryId, It.Is<City>(c => c.Name == "Salvador" && c.Country == country), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}