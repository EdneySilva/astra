using Astra.Domain;
using Astra.Domain.Abstractions;

namespace Astra.Manager
{
    public interface ICityManager
    {
        Task<Result<City>> AddCityAsync(int countryId, City city, CancellationToken cancellationToken = default);
        Task<Result<bool>> CountryContainsCityAsync(int countryId, City city, CancellationToken cancellationToken = default);
        Task<Result<City?>> FindCityByIdAsync(int countryId, int cityId, CancellationToken cancellationToken = default);
        Task<Result<PageResult<City>>> GetCitiesFromCountryAsync(int countryId, CancellationToken cancellationToken = default);
        Task<Result<City?>> RemoveCityAsync(int countryId, City city, CancellationToken cancellationToken = default);
    }
}