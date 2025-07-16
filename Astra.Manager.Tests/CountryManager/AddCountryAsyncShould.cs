using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Astra.Manager.Tests.CountryManager
{
    public class AddCountryAsyncShould
    {
        private readonly Manager.CountryManager countryManager;

        private readonly Mock<ICountryRepository> _countryRepository;

        public AddCountryAsyncShould()
        {
            _countryRepository = new Mock<ICountryRepository>();
            var _logger = new Mock<ILogger<Manager.CountryManager>>();
            var _cityRepository = new Mock<ICityRepository>();
            countryManager = new Manager.CountryManager(_logger.Object, _countryRepository.Object, null);
        }

        [Fact]
        public async Task Add_country_when_name_is_unique()
        {
            var country = new Country
            {
                Name = "Brazil"
            };
            var result = await countryManager.AddCountryAsync(country);

            _countryRepository.Verify(a => a.FindByNameAsync(It.Is<string>(c => c == country.Name), It.IsAny<CancellationToken>()), Times.Once());
            _countryRepository.Verify(a => a.AddAsync(It.Is<Country>(c => c == country), It.IsAny<CancellationToken>()), Times.Once());

            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(country.Name);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Not_add_country_when_name_is_not_unique()
        {
            var country = new Country
            {
                Name = "Brazil"
            };

            _countryRepository.Setup(x => x.FindByNameAsync(It.Is<string>(c => c == country.Name), It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            var result = await countryManager.AddCountryAsync(country);

            _countryRepository.Verify(a => a.FindByNameAsync(It.Is<string>(c => c == country.Name), It.IsAny<CancellationToken>()), Times.Once());
            _countryRepository.Verify(a => a.AddAsync(It.Is<Country>(c => c == country), It.IsAny<CancellationToken>()), Times.Never());

            result.Value.Should().BeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CountryNameConflict");
        }
    }
}