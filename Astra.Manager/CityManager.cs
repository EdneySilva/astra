using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using Microsoft.Extensions.Logging;
using Astra.Manager.Data.City;

namespace Astra.Manager
{
    public class CityManager : ICityManager
    {
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<CityManager> _logger;

        public CityManager(ICityRepository cityRepository, ILogger<CityManager> logger)
        {
            _cityRepository = cityRepository;
            _logger = logger;
        }

        public async Task<Result<City>> AddCityAsync(int countryId, City city, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Adding city: {Name} in province {Province}", city.Name, city.Province);

            var exists = await CountryContainsCityAsync(countryId, city, cancellationToken);
            if (exists.Value)
            {
                _logger.LogWarning("Duplicated city found: {Name}/{Province}", city.Name, city.Province);
                return Result<City>.Failure("DuplicatedCity");
            }

            await _cityRepository.AddAsync(city, cancellationToken);
            _logger.LogInformation("City added: {Name}/{Province} (Id: {Id})", city.Name, city.Province, city.Id);
            return Result<City>.Success(city);
        }

        public async Task<Result<City?>> RemoveCityAsync(int countryId, City city, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Removing city: {Name}/{Province}", city.Name, city.Province);

            var result = await FindCityByIdAsync(countryId, city.Id, cancellationToken);
            if (result.IsFailure)
                return result;

            await _cityRepository.DeleteCity(result.Value!, cancellationToken);
            _logger.LogInformation("City removed: {Name}/{Province}", city.Name, city.Province);
            return Result<City?>.Success(city);
        }

        public async Task<Result<bool>> CountryContainsCityAsync(int countryId, City city, CancellationToken cancellationToken = default)
        {
            var found = await _cityRepository.FindAync(CityQueries.CityOnCountry(countryId, city), cancellationToken);
            return Result<bool>.Success(found is not null);
        }

        public async Task<Result<City?>> FindCityByIdAsync(int countryId, int cityId, CancellationToken cancellationToken = default)
        {
            var city = await _cityRepository.FindByIdAsync(cityId, cancellationToken);
            if (city == null)
                return Result<City?>.Failure("CityNotFound");

            if (city.Country.Id != countryId)
                return Result<City?>.Failure("CityNotFoundOnCountry");

            return Result<City?>.Success(city);
        }

        public async Task<Result<PageResult<City>>> GetCitiesFromCountryAsync(int countryId, CancellationToken cancellationToken = default)
        {
            var result = await _cityRepository.GetAllAsync(CityQueries.AllOnCountry(countryId), PageRequest.First(), cancellationToken);
            return Result<PageResult<City>>.Success(result);
        }
    }
}
