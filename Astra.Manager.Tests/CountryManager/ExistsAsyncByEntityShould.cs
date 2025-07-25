using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using Astra.Domain.Specifications;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class ExistsAsyncByEntityShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;

        public ExistsAsyncByEntityShould()
        {
            _countryRepository = new Mock<ICountryRepository>();
            var _logger = new Mock<ILogger<Manager.CountryManager>>();
            var _cityManager = new Mock<ICityManager>();
            countryManager = new Manager.CountryManager(_logger.Object, _countryRepository.Object, _cityManager.Object);
        }

        [Fact]
        public async Task Returns_true_when_country_with_same_name_exists()
        {
            var inputCountry = new Country { Name = "Brazil", Code = "BRL" };
            var existingCountry = new Country { Name = "Brazil", Code = "BRL" };

            _countryRepository
                .Setup(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCountry);

            var result = await countryManager.ExistsAsync(inputCountry, CancellationToken.None);

            _countryRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once());
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Fact]
        public async Task Returns_false_when_country_with_same_name_does_not_exist()
        {
            var inputCountry = new Country { Name = "Atlantis", Code = "ATL" };

            _countryRepository
                .Setup(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.ExistsAsync(inputCountry, CancellationToken.None);

            _countryRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once());
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse();
        }
    }
}