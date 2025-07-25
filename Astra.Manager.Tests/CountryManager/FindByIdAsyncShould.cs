using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class FindByIdAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;

        public FindByIdAsyncShould()
        {
            _countryRepository = new Mock<ICountryRepository>();
            var _logger = new Mock<ILogger<Manager.CountryManager>>();
            var _cityManager = new Mock<ICityManager>();
            countryManager = new Manager.CountryManager(_logger.Object, _countryRepository.Object, _cityManager.Object);
        }

        [Fact]
        public async Task Returns_country_when_it_exists()
        {
            int countryId = 1;
            var country = new Country { Id = countryId, Name = "Brazil", Code = "BRL" };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            var result = await countryManager.FindByIdAsync(countryId, CancellationToken.None);

            _countryRepository.Verify(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()), Times.Once());
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("Brazil");
        }

        [Fact]
        public async Task Returns_null_when_country_does_not_exist()
        {
            int countryId = 999;

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.FindByIdAsync(countryId, CancellationToken.None);

            _countryRepository.Verify(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()), Times.Once());
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();
        }
    }
}