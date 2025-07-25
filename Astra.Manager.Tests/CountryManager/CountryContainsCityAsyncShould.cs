using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class CountryContainsCityAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;
        private readonly Mock<ILogger<Manager.CountryManager>> _logger;
        private readonly Mock<Manager.ICityManager> _cityManagerMock;

        public CountryContainsCityAsyncShould()
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
            var city = new City { Name = "Recife", Country = new Country() { Id = countryId }, Province = "PE" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.CountryContainsCityAsync(countryId, city, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CountryNotFound");

            _cityManagerMock.Verify(m => m.CountryContainsCityAsync(It.IsAny<int>(), It.IsAny<City>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Returns_true_when_country_contains_city()
        {
            int countryId = 2;
            var country = new Country { Id = countryId, Name = "Brazil" };
            var city = new City { Name = "Fortaleza", Country = country, Province = "CE" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            _cityManagerMock
                .Setup(m => m.CountryContainsCityAsync(countryId, city, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await countryManager.CountryContainsCityAsync(countryId, city, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            _cityManagerMock.Verify(m => m.CountryContainsCityAsync(countryId, city, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Returns_false_when_country_does_not_contain_city()
        {
            int countryId = 3;
            var country = new Country { Id = countryId, Name = "United States of America" };
            var city = new City { Name = "Gotham", Country = country, Province = "NY" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            _cityManagerMock
                .Setup(m => m.CountryContainsCityAsync(countryId, city, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(false));

            var result = await countryManager.CountryContainsCityAsync(countryId, city, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse();

            _cityManagerMock.Verify(m => m.CountryContainsCityAsync(countryId, city, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}