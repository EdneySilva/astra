using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using Astra.Domain.Specifications;
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
            var _cityManager = new Mock<ICityManager>();
            countryManager = new Manager.CountryManager(_logger.Object, _countryRepository.Object, _cityManager.Object);
        }

        [Fact]
        public async Task Returns_failure_when_name_is_not_provided()
        {
            var country = new Country
            {
                Code = "BRL"
            };
            var result = await countryManager.AddCountryAsync(country);

            _countryRepository.Verify(a => a.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Never());
            _countryRepository.Verify(a => a.AddAsync(It.Is<Country>(c => c == country), It.IsAny<CancellationToken>()), Times.Never());

            result.Value.Should().BeNull();
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("NameIsRequired");
        }
        
        [Fact]
        public async Task Returns_failure_when_code_is_not_provided()
        {
            var country = new Country
            {
                Name = "BRAZIL",
            };
            var result = await countryManager.AddCountryAsync(country);

            _countryRepository.Verify(a => a.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Never());
            _countryRepository.Verify(a => a.AddAsync(It.Is<Country>(c => c == country), It.IsAny<CancellationToken>()), Times.Never());

            result.Value.Should().BeNull();
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CodeIsRequired");
        }

        [Fact]
        public async Task Add_country_when_name_is_unique()
        {
            var country = new Country
            {
                Name = "Brazil",
                Code = "BRL"
            };
            var result = await countryManager.AddCountryAsync(country);

            _countryRepository.Verify(a => a.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once());
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
                Name = "Brazil",
                Code = "BRL"
            };

            _countryRepository.Setup(x => x.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(country);

            var result = await countryManager.AddCountryAsync(country);

            _countryRepository.Verify(a => a.FindAsync(It.IsAny<ISpecification<Country>>(), It.IsAny<CancellationToken>()), Times.Once());
            _countryRepository.Verify(a => a.AddAsync(It.Is<Country>(c => c == country), It.IsAny<CancellationToken>()), Times.Never());

            result.Value.Should().BeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("CountryNameConflict");
        }
    }
}