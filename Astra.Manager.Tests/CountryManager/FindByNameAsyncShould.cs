using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using Astra.Domain.Specifications;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class FindByNameAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;

        public FindByNameAsyncShould()
        {
            _countryRepository = new Mock<ICountryRepository>();
            var _logger = new Mock<ILogger<Manager.CountryManager>>();
            var _cityManager = new Mock<ICityManager>();
            countryManager = new Manager.CountryManager(_logger.Object, _countryRepository.Object, _cityManager.Object);
        }

        [Fact]
        public async Task Returns_failure_when_name_is_invalid()
        {
            var invalidNames = new[] { null, "", "   " };

            foreach (var name in invalidNames)
            {
                var result = await countryManager.FindByNameAsync(name, CancellationToken.None);

                result.IsFailure.Should().BeTrue();
                result.Error.Should().Be("InvalidName");
                result.Value.Should().BeNull();
            }

            _countryRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Returns_country_when_found()
        {
            var countryName = "Brazil";
            var country = new Country { Id = 1, Name = countryName, Code = "BRL" };

            _countryRepository
                .Setup(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            var result = await countryManager.FindByNameAsync(countryName, CancellationToken.None);

            _countryRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once());
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be(countryName);
        }

        [Fact]
        public async Task Returns_null_when_country_not_found()
        {
            var countryName = "Atlantis";

            _countryRepository
                .Setup(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.FindByNameAsync(countryName, CancellationToken.None);

            _countryRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once());
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();
        }
    }

}