using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using Astra.Domain.Specifications;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class DeleteCountryAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;

        public DeleteCountryAsyncShould()
        {
            _countryRepository = new Mock<ICountryRepository>();
            var _logger = new Mock<ILogger<Manager.CountryManager>>();
            var _cityManager = new Mock<ICityManager>();
            countryManager = new Manager.CountryManager(_logger.Object, _countryRepository.Object, _cityManager.Object);
        }

        [Fact]
        public async Task Returns_failure_when_name_is_missing()
        {
            var country = new Country { Id = 1, Name = string.Empty, Code = "BRL" };

            var result = await countryManager.DeleteCountryAsync(country, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("NameIsRequired");

            _countryRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Never);
            _countryRepository.Verify(r => r.DeleteAsync(It.IsAny<Country>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Returns_failure_when_country_not_found()
        {
            var country = new Country { Id = 2, Name = "Atlantis", Code = "ATL" };

            _countryRepository
                .Setup(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.DeleteCountryAsync(country, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CountryNotFound");

            _countryRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once);
            _countryRepository.Verify(r => r.DeleteAsync(It.IsAny<Country>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deletes_country_successfully()
        {
            var country = new Country { Id = 3, Name = "Brazil", Code = "BRL" };
            var existing = new Country { Id = 3, Name = "Brazil", Code = "BRL" };

            _countryRepository
                .Setup(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            var result = await countryManager.DeleteCountryAsync(country, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(country);

            _countryRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once);
            _countryRepository.Verify(r => r.DeleteAsync(country, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}