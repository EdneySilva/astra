using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class FindCityByIdAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;
        private readonly Mock<ILogger<Manager.CountryManager>> _logger;
        private readonly Mock<Manager.ICityManager> _cityManagerMock;

        public FindCityByIdAsyncShould()
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
            int cityId = 10;

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.FindCityByIdAsync(countryId, cityId, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CountryNotFound");

            _cityManagerMock.Verify(m => m.FindCityByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Returns_city_when_found()
        {
            int countryId = 2;
            int cityId = 101;
            var country = new Country { Id = countryId, Name = "Brazil" };
            var city = new City { Id = cityId, Name = "Recife", Country = country, Province = "PE" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            _cityManagerMock
                .Setup(m => m.FindCityByIdAsync(countryId, cityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<City?>.Success(city));

            var result = await countryManager.FindCityByIdAsync(countryId, cityId, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(cityId);
            result.Value!.Name.Should().Be("Recife");

            _cityManagerMock.Verify(m => m.FindCityByIdAsync(countryId, cityId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Returns_null_when_city_not_found()
        {
            int countryId = 3;
            int cityId = 999;
            var country = new Country { Id = countryId, Name = "Brazil" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            _cityManagerMock
                .Setup(m => m.FindCityByIdAsync(countryId, cityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<City?>.Success(null));

            var result = await countryManager.FindCityByIdAsync(countryId, cityId, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();

            _cityManagerMock.Verify(m => m.FindCityByIdAsync(countryId, cityId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}