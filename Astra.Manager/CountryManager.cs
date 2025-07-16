using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
namespace Astra.Manager
{
    public interface IQueryContextProvider
    {
        PaginationContext GetCurrentContext();
    }

    public struct PaginationContext
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

    }

    public class CountryManager
    {
        private readonly ILogger<CountryManager> _logger;
        private ICountryRepository _countryRespository;
        private readonly IQueryContextProvider _paginationContextProvider;
        private ICityRepository _cityRepository;

        public CountryManager(ILogger<CountryManager> logger, ICountryRepository repository, ICityRepository cityRepository, IQueryContextProvider paginationContextProvider)
        {
            _logger = logger;
            _countryRespository = repository;
            _cityRepository = cityRepository;
            _paginationContextProvider = paginationContextProvider;
        }

        public async Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Starting ExistsAsync({@country})", id);
            var result = await _countryRespository.FindByIdAsync(id, cancellationToken);
            var exist = result is not null;
            _logger.LogTrace("ExistsAsync completed with result: {result}", exist);
            if (result is not null)
                return Result<bool>.Success(true);
            return Result<bool>.Success(false);
        }
        
        public async Task<Result<bool>> ExistsAsync(Country country, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Starting ExistsAsync({@country})", country);
            var result = await _countryRespository.FindByNameAsync(country.Name, cancellationToken);
            var exist = result is not null;
            _logger.LogTrace("ExistsAsync completed with result: {result}", exist);
            if (result is not null)
                return Result<bool>.Success(true);
            return Result<bool>.Success(false);
        }

        public async Task<Result<Country>> AddCountryAsync(Country country, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Starting AddCountryAsync({@country})", country);
            if (string.IsNullOrEmpty(country.Name))
            {
                _logger.LogDebug("Validation failed: NameIsRequired");
                return Result<Country>.Failure("NameIsRequired");
            }

            var existingCountry = await this._countryRespository.FindByNameAsync(country.Name, cancellationToken);
            if (existingCountry is not null)
            {
                _logger.LogDebug("CountryNameConflict {name}", country.Name);
                return Result<Country>.Failure("CountryNameConflict");
            }
            country.Id = Guid.NewGuid();
            await this._countryRespository.AddAsync(country, cancellationToken);
            _logger.LogDebug("CountryAdded {name} with Id {id}", country.Name, country.Id);
            _logger.LogTrace("Result AddCountryAsync {@country}", country);
            return Result<Country>.Success(country);
        }

        public async Task<Result<PageResult<City>>> FindCitiesAsync(Guid countryId, CancellationToken cancellationToken = default)
        {
            var pagination = _paginationContextProvider.GetCurrentContext();
            var result = await _cityRepository.GetAllAsync(countryId, PageRequest.First(), cancellationToken);
            return result;
        }

        public async Task<Result<IEnumerable<Country>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Starting GetAllAsync()");
            var data = (await _countryRespository.GetAllAsync(cancellationToken).ConfigureAwait(false)).ToList();
            _logger.LogDebug("Found {@count} countries", data.Count);
            _logger.LogTrace("GetAllAsync completed");
            return Result<IEnumerable<Country>>.Success(data);
        }
        
        public async Task<Result<IEnumerable<Country>>> GetCitiesFromCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Starting GetCitiesFromCountryAsync({id})", countryId);
            var data = (await _countryRespository.GetAllAsync(cancellationToken).ConfigureAwait(false)).ToList();
            _logger.LogDebug("Found {@count} cities", data.Count);
            _logger.LogTrace("GetCitiesFromCountryAsync completed");
            return Result<IEnumerable<Country>>.Success(data);
        }

        public async Task<Result<Country?>> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Starting FindByIdAsync({@name})", id);
            _logger.LogDebug("Finding country by name {id} ", id);
            var data = await _countryRespository.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (data == null)
                _logger.LogInformation("Country not found by name {id}", id);
            _logger.LogInformation("Found {country}", data);
            _logger.LogTrace("FindByNameAsync completed");
            return Result<Country?>.Success(data);
        }
        
        public async Task<Result<Country?>> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Starting FindByNameAsync({@name})", name);
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("ArgumentNull(name)");
                return Result<Country?>.Failure("InvalidName");
            }

            _logger.LogDebug("Finding country by name {name} ", name);
            var data = await _countryRespository.FindByNameAsync(name, cancellationToken).ConfigureAwait(false);
            if (data == null)
                _logger.LogInformation("Country not found by name {name}", name);
            _logger.LogInformation("Found {country}", data);
            _logger.LogTrace("FindByNameAsync completed");
            return Result<Country?>.Success(data);
        }

        public async Task<Result<Country>> UpdateCountryAsync(Country country, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Starting DeleteCountryAsync({@country})", country);
            if (string.IsNullOrEmpty(country.Name))
            {
                _logger.LogDebug("Validation failed: NameIsRequired");
                return Result<Country>.Failure("NameIsRequired");
            }

            var existingCountry = await this._countryRespository.FindByNameAsync(country.Name, cancellationToken);
            if (existingCountry is null)
            {
                _logger.LogDebug("CountryNotFound {name}", country.Name);
                return Result<Country>.Failure("CountryNotFound");
            }
            await this._countryRespository.DeleteAsync(country, cancellationToken);
            _logger.LogDebug("CountryDeleted {name}", country.Name);
            _logger.LogTrace("Result DeleteCountryAsync {@country}", country);
            return Result<Country>.Success(country);
        }
        
        public async Task<Result<Country>> DeleteCountryAsync(Country country, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Starting DeleteCountryAsync({@country})", country);
            if (string.IsNullOrEmpty(country.Name))
            {
                _logger.LogDebug("Validation failed: NameIsRequired");
                return Result<Country>.Failure("NameIsRequired");
            }

            var existingCountry = await this._countryRespository.FindByNameAsync(country.Name, cancellationToken);
            if (existingCountry is null)
            {
                _logger.LogDebug("CountryNotFound {name}", country.Name);
                return Result<Country>.Failure("CountryNotFound");
            }
            await this._countryRespository.DeleteAsync(country, cancellationToken);
            _logger.LogDebug("CountryDeleted {name}", country.Name);
            _logger.LogTrace("Result DeleteCountryAsync {@country}", country);
            return Result<Country>.Success(country);
        }

    }
}
