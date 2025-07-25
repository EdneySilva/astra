using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class GetCitiesFromCountryAsyncShould
    {
        private readonly Manager.CountryManager countryManager;
        private readonly Mock<ICountryRepository> _countryRepository;
        private readonly Mock<ILogger<Manager.CountryManager>> _logger;
        private readonly Mock<Manager.ICityManager> _cityManagerMock;

        public GetCitiesFromCountryAsyncShould()
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

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Country?)null);

            var result = await countryManager.GetCitiesFromCountryAsync(countryId, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CountryNotFound");

            _cityManagerMock.Verify(m => m.GetCitiesFromCountryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Returns_page_result_with_cities_when_country_exists()
        {
            int countryId = 2;
            var country = new Country { Id = countryId, Name = "Brazil" };
            var cities = new List<City>
            {
                new City { Id = 1, Name = "Rio de Janeiro", Country = country, Province = "RJ" },
                new City { Id = 2, Name = "São Paulo", Country = country, Province = "SP" }
            };

            var pageResult = new PageResult<City>
            {
                Page = 1,
                PageSize = 10,
                TotalItemsFound = 2,
                Items = cities
            };

            _countryRepository
                .Setup(r => r.FindByIdAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            _cityManagerMock
                .Setup(m => m.GetCitiesFromCountryAsync(countryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PageResult<City>>.Success(pageResult));

            var result = await countryManager.GetCitiesFromCountryAsync(countryId, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Items.Should().HaveCount(2);
            result.Value.TotalItemsFound.Should().Be(2);
            result.Value.Page.Should().Be(1);
            result.Value.PageSize.Should().Be(10);

            _cityManagerMock.Verify(m => m.GetCitiesFromCountryAsync(countryId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}