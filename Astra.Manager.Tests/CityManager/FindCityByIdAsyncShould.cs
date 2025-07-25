using Astra.Domain.Abstractions.Data;
using Astra.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Astra.Manager.Tests.CityManager
{
    public class FindCityByIdAsyncShould
    {
        private readonly Mock<ICityRepository> _cityRepository;
        private readonly Mock<ILogger<Manager.CityManager>> _logger;
        private readonly Manager.CityManager _cityManager;

        public FindCityByIdAsyncShould()
        {
            _cityRepository = new Mock<ICityRepository>();
            _logger = new Mock<ILogger<Manager.CityManager>>();
            _cityManager = new Manager.CityManager(_cityRepository.Object, _logger.Object);
        }

        [Fact]
        public async Task Returns_failure_when_city_not_found()
        {
            int cityId = 10;
            int countryId = 1;

            _cityRepository
                .Setup(r => r.FindByIdAsync(cityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((City?)null);

            var result = await _cityManager.FindCityByIdAsync(countryId, cityId, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CityNotFound");
        }

        [Fact]
        public async Task Returns_failure_when_city_belongs_to_another_country()
        {
            int requestedCountryId = 1;
            int actualCountryId = 2;
            int cityId = 20;

            var city = new City
            {
                Id = cityId,
                Name = "Florida",
                Province = "MA",
                Country = new Country { Id = actualCountryId }
            };

            _cityRepository
                .Setup(r => r.FindByIdAsync(cityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(city);

            var result = await _cityManager.FindCityByIdAsync(requestedCountryId, cityId, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CityNotFoundOnCountry");
        }

        [Fact]
        public async Task Returns_city_when_found_and_belongs_to_country()
        {
            int countryId = 3;
            int cityId = 30;

            var city = new City
            {
                Id = cityId,
                Name = "Curitiba",
                Province = "PR",
                Country = new Country { Id = countryId }
            };

            _cityRepository
                .Setup(r => r.FindByIdAsync(cityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(city);

            var result = await _cityManager.FindCityByIdAsync(countryId, cityId, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(cityId);
            result.Value!.Country.Id.Should().Be(countryId);
        }
    }

}
