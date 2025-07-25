using Astra.Domain.Abstractions.Data;
using Astra.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Astra.Domain.Specifications;

namespace Astra.Manager.Tests.CityManager
{
    public class AddCityAsyncShould
    {
        private readonly Mock<ICityRepository> _cityRepository;
        private readonly Mock<ILogger<Manager.CityManager>> _logger;
        private readonly Manager.CityManager _cityManagerMock;

        public AddCityAsyncShould()
        {
            _cityRepository = new Mock<ICityRepository>();
            _logger = new Mock<ILogger<Manager.CityManager>>();
            _cityManagerMock = new Manager.CityManager(_cityRepository.Object, _logger.Object);
        }

        [Fact]
        public async Task Returns_failure_when_city_already_exists()
        {
            int countryId = 1;
            var city = new City { Id = 0, Name = "São Paulo", Province = "SP", Country = new Country { Id = countryId } };

            _cityRepository
                .Setup(m => m.FindAync(It.IsAny<ISpecification<City>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(city);

            var result = await _cityManagerMock.AddCityAsync(countryId, city, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("DuplicatedCity");

            _cityRepository.Verify(r => r.AddAsync(It.IsAny<City>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Adds_city_successfully_when_it_does_not_exist()
        {
            int countryId = 2;
            var city = new City { Id = 0, Name = "Campinas", Province = "SP", Country = new Country { Id = countryId } };

            _cityRepository
                .Setup(m => m.FindAync(It.IsAny<ISpecification<City>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((City?)null);

            _cityRepository
                .Setup(r => r.AddAsync(city, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _cityManagerMock.AddCityAsync(countryId, city, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(city);

            _cityRepository.Verify(r => r.AddAsync(city, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
