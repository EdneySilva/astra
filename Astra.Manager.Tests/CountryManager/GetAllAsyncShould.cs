using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using Astra.Domain.Specifications;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class GetAllAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;

        public GetAllAsyncShould()
        {
            _countryRepository = new Mock<ICountryRepository>();
            var _logger = new Mock<ILogger<Manager.CountryManager>>();
            var _cityManager = new Mock<ICityManager>();
            countryManager = new Manager.CountryManager(_logger.Object, _countryRepository.Object, _cityManager.Object);
        }

        [Fact]
        public async Task Returns_all_countries_successfully()
        {
            var countries = new List<Country>
            {
                new Country { Id = 1, Name = "Brazil", Code = "BRL" },
                new Country { Id = 2, Name = "Argentina", Code = "ARS" }
            };

            _countryRepository
                .Setup(r => r.GetAllAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(countries);

            var result = await countryManager.GetAllAsync(CancellationToken.None);

            _countryRepository.Verify(r => r.GetAllAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once());
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().Contain(c => c.Name == "Brazil");
            result.Value.Should().Contain(c => c.Name == "Argentina");
        }

        [Fact]
        public async Task Returns_empty_list_when_no_countries_exist()
        {
            _countryRepository
                .Setup(r => r.GetAllAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Country>());

            var result = await countryManager.GetAllAsync(CancellationToken.None);

            _countryRepository.Verify(r => r.GetAllAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once());
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}