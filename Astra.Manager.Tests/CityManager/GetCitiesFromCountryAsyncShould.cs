using Astra.Domain.Abstractions.Data;
using Astra.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Astra.Domain.Specifications;
using Astra.Domain.Abstractions;

namespace Astra.Manager.Tests.CityManager
{
    public class GetCitiesFromCountryAsyncShould
    {
        private readonly Mock<ICityRepository> _cityRepository;
        private readonly Mock<ILogger<Manager.CityManager>> _logger;
        private readonly Manager.CityManager _cityManager;

        public GetCitiesFromCountryAsyncShould()
        {
            _cityRepository = new Mock<ICityRepository>();
            _logger = new Mock<ILogger<Manager.CityManager>>();
            _cityManager = new Manager.CityManager(_cityRepository.Object, _logger.Object);
        }

        [Fact]
        public async Task Returns_page_result_of_cities_from_given_country()
        {
            int countryId = 1;
            var cities = new List<City>
            {
                new City { Id = 1, Name = "Natal", Country = new Country{ Id = countryId }, Province = "RGN" },
                new City { Id = 2, Name = "Mossoró", Country = new Country{ Id = countryId }, Province = "RGN" }
            };

            var pageResult = new PageResult<City>
            {
                Page = 1,
                PageSize = 10,
                TotalItemsFound = 2,
                Items = cities
            };

            _cityRepository
                .Setup(r => r.GetAllAsync(It.IsAny<ISpecification<City>>(), It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pageResult);

            var result = await _cityManager.GetCitiesFromCountryAsync(countryId, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Items.Should().HaveCount(2);
            result.Value.TotalItemsFound.Should().Be(2);
            result.Value.Page.Should().Be(1);
            result.Value.PageSize.Should().Be(10);
        }
    }

}
