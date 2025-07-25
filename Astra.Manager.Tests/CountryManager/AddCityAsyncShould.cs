using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class AddCityAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;
        private readonly Mock<ILogger<Manager.CountryManager>> _logger;
        private readonly Mock<Manager.ICityManager> _cityManagerMock;

        public AddCityAsyncShould()
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
            var city = new City { Name = "São Paulo", Country = new Country { Id = countryId }, Province = "SP" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.AddCityAsync(countryId, city, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CountryNotFound");

            _cityManagerMock.Verify(m => m.AddCityAsync(It.IsAny<int>(), It.IsAny<City>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Adds_city_successfully_when_country_exists()
        {
            int countryId = 1;
            var country = new Country { Id = countryId, Name = "Brazil", Code = "BRL" };
            var city = new City { Name = "São Paulo", Province = "SP", Country = country };
            var createdCity = new City { Id = 100, Name = "São Paulo", Country = country, Province = "SP" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);
            var cityResult = Result<City>.Success(createdCity);
            _cityManagerMock
                .Setup(m => m.AddCityAsync(countryId, It.IsAny<City>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cityResult);

            var result = await countryManager.AddCityAsync(countryId, city, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(100);
            result.Value!.Country.Should().Be(country);

            _cityManagerMock.Verify(m => m.AddCityAsync(countryId, It.Is<City>(c => c.Name == "São Paulo" && c.Country == country), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}