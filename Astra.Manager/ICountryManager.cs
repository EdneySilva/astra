using Astra.Domain;
using Astra.Domain.Abstractions;

namespace Astra.Manager
{
    public interface ICountryManager
    {
        Task<Result<City>> AddCityAsync(int countryId, City city, CancellationToken cancellationToken = default);
        Task<Result<Country>> AddCountryAsync(Country country, CancellationToken cancellationToken = default);
        Task<Result<bool>> CountryContainsCityAsync(int countryId, City city, CancellationToken cancellationToken = default);
        Task<Result<Country>> DeleteCountryAsync(Country country, CancellationToken cancellationToken = default);
        Task<Result<bool>> ExistsAsync(Country country, CancellationToken cancellationToken);
        Task<Result<bool>> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<Result<Country?>> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<Country?>> FindByNameAsync(string? name, CancellationToken cancellationToken = default);
        Task<Result<City?>> FindCityByIdAsync(int countryId, int cityId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<Country>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<PageResult<City>>> GetCitiesFromCountryAsync(int countryId, CancellationToken cancellationToken = default);
        Task<Result<City?>> RemoveCityAsync(int countryId, City city, CancellationToken cancellationToken = default);
        Task<Result<Country>> UpdateCountryAsync(Country country, CancellationToken cancellationToken = default);
    }
}