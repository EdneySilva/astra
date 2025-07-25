using Astra.Domain.Abstractions.Data;
using Astra.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Astra.Domain.Specifications;

namespace Astra.Manager.Tests.CityManager
{
    public class CountryContainsCityAsyncShould
    {
        private readonly Mock<ICityRepository> _cityRepository;
        private readonly Mock<ILogger<Manager.CityManager>> _logger;
        private readonly Manager.CityManager _cityManager;

        public CountryContainsCityAsyncShould()
        {
            _cityRepository = new Mock<ICityRepository>();
            _logger = new Mock<ILogger<Manager.CityManager>>();
            _cityManager = new Manager.CityManager(_cityRepository.Object, _logger.Object);
        }

        [Fact]
        public async Task Returns_true_when_city_exists_in_country()
        {
            int countryId = 1;
            var city = new City { Name = "Fortaleza", Province = "CE", Country = new Country { Id = countryId } };
            var foundCity = new City { Id = 100, Name = "Fortaleza", Province = "CE", Country = new Country { Id = countryId } };

            _cityRepository
                .Setup(r => r.FindAync(It.IsAny<ISpecification<City>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(foundCity);

            var result = await _cityManager.CountryContainsCityAsync(countryId, city, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Fact]
        public async Task Returns_false_when_city_does_not_exist_in_country()
        {
            int countryId = 2;
            var city = new City { Name = "Gotham", Province = "NY", Country = new Country { Id = countryId } };

            _cityRepository
                .Setup(r => r.FindAync(It.IsAny<ISpecification<City>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((City?)null);

            var result = await _cityManager.CountryContainsCityAsync(countryId, city, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse();
        }
    }
}
