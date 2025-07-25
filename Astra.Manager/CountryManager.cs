using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using Microsoft.Extensions.Logging;
using Astra.Manager.Data.Country;

namespace Astra.Manager
{
    public class CountryManager : ICountryManager
    {
        private readonly ILogger<CountryManager> _logger;
        private ICountryRepository _countryRespository;
        private readonly ICityManager _cityManager;

        public CountryManager(ILogger<CountryManager> logger, ICountryRepository repository, ICityManager cityManager)
        {
            _logger = logger;
            _countryRespository = repository;
            _cityManager = cityManager;
        }

        public async Task<Result<bool>> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Checking if country exists (Id: {CountryId})", id);
            var result = await _countryRespository.FindByIdAsync(id, cancellationToken);
            return Result<bool>.Success(result is not null);
        }

        public async Task<Result<bool>> ExistsAsync(Country country, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Checking if country exists (Name: {Name})", country.Name);
            var result = await _countryRespository.FindAsync(CountryQueries.WithName(country.Name), cancellationToken);
            return Result<bool>.Success(result is not null);
        }

        public async Task<Result<Country>> AddCountryAsync(Country country, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Adding new country: {Name}", country.Name);

            if (string.IsNullOrWhiteSpace(country.Name))
                return Result<Country>.Failure("NameIsRequired");

            if (string.IsNullOrWhiteSpace(country.Code))
                return Result<Country>.Failure("CodeIsRequired");

            var existing = await _countryRespository.FindAsync(CountryQueries.WithName(country.Name), cancellationToken);
            if (existing is not null)
                return Result<Country>.Failure("CountryNameConflict");

            await _countryRespository.AddAsync(country, cancellationToken);
            _logger.LogInformation("Country added: {Name} (Id: {Id})", country.Name, country.Id);
            return Result<Country>.Success(country);
        }

        public async Task<Result<IEnumerable<Country>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var data = (await _countryRespository.GetAllAsync(CountryQueries.All(), cancellationToken)).ToList();
            _logger.LogInformation("Retrieved {Count} countries", data.Count);
            return Result<IEnumerable<Country>>.Success(data);
        }

        public async Task<Result<Country?>> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Finding country by ID: {Id}", id);
            var data = await _countryRespository.FindByIdAsync(id, cancellationToken);
            if (data == null)
                _logger.LogWarning("Country not found (Id: {Id})", id);
            else
                _logger.LogInformation("Country found: {Name}", data.Name);

            return Result<Country?>.Success(data);
        }

        public async Task<Result<Country?>> FindByNameAsync(string? name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Invalid country name provided");
                return Result<Country?>.Failure("InvalidName");
            }

            _logger.LogDebug("Finding country by name: {Name}", name);
            var data = await _countryRespository.FindAsync(CountryQueries.WithName(name), cancellationToken);
            if (data == null)
                _logger.LogWarning("Country not found (Name: {Name})", name);
            else
                _logger.LogInformation("Country found: {Name}", data.Name);

            return Result<Country?>.Success(data);
        }

        public async Task<Result<Country>> UpdateCountryAsync(Country country, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Updating country: {Name}", country.Name);

            if (string.IsNullOrWhiteSpace(country.Name))
                return Result<Country>.Failure("NameIsRequired");

            var existing = await _countryRespository.FindAsync(CountryQueries.WithName(country.Name), cancellationToken);
            if (existing is null)
                return Result<Country>.Failure("CountryNotFound");

            await _countryRespository.UpdateAsync(country, cancellationToken);
            _logger.LogInformation("Country updated: {Name}", country.Name);
            return Result<Country>.Success(country);
        }

        public async Task<Result<Country>> DeleteCountryAsync(Country country, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Deleting country: {Name}", country.Name);

            if (string.IsNullOrWhiteSpace(country.Name))
                return Result<Country>.Failure("NameIsRequired");

            var existing = await _countryRespository.FindAsync(CountryQueries.WithName(country.Name), cancellationToken);
            if (existing is null)
                return Result<Country>.Failure("CountryNotFound");

            await _countryRespository.DeleteAsync(country, cancellationToken);
            _logger.LogInformation("Country deleted: {Name}", country.Name);
            return Result<Country>.Success(country);
        }

        public async Task<Result<City>> AddCityAsync(int countryId, City city, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Adding city '{CityName}' to country {CountryId}", city.Name, countryId);
            return await DoIfCountryExists(countryId, (context, country, param, token) =>
            {
                city.Country = country;
                return context._cityManager.AddCityAsync(countryId, city, token);
            }, city, cancellationToken);
        }

        public async Task<Result<City?>> RemoveCityAsync(int countryId, City city, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Removing city '{CityName}' from country {CountryId}", city.Name, countryId);
            return await DoIfCountryExists(countryId, (context, country, param, token) =>
            {
                city.Country = country;
                return context._cityManager.RemoveCityAsync(countryId, city, token);
            }, city, cancellationToken);
        }

        public async Task<Result<bool>> CountryContainsCityAsync(int countryId, City city, CancellationToken cancellationToken = default)
        {
            return await DoIfCountryExists(countryId, (context, country, param, token) =>
            {
                return context._cityManager.CountryContainsCityAsync(country.Id, city, token);
            }, city, cancellationToken);
        }

        public async Task<Result<City?>> FindCityByIdAsync(int countryId, int cityId, CancellationToken cancellationToken = default)
        {
            return await DoIfCountryExists(countryId, (context, country, param, token) =>
            {
                return context._cityManager.FindCityByIdAsync(country.Id, cityId, token);
            }, cityId, cancellationToken);
        }

        public async Task<Result<PageResult<City>>> GetCitiesFromCountryAsync(int countryId, CancellationToken cancellationToken = default)
        {
            return await DoIfCountryExists(countryId, (context, country, param, token) =>
            {
                return context._cityManager.GetCitiesFromCountryAsync(country.Id, token);
            }, countryId, cancellationToken);
        }

        private async Task<Result<T>> DoIfCountryExists<T, TP>(int countryId, Func<CountryManager, Country, TP, CancellationToken, Task<Result<T>>> operation, TP parameter, CancellationToken cancellationToken = default)
        {
            var country = await FindByIdAsync(countryId, cancellationToken);
            if (country.Value is null)
            {
                _logger.LogDebug("Validation failed: CountryNotFound");
                return Result<T>.Failure("CountryNotFound");
            }
            return await operation(this, country.Value, parameter, cancellationToken);
        }
    }
}
