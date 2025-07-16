namespace Astra.Domain.Abstractions.Data
{
    public interface ICountryRepository
    {
        Task AddAsync(Country country, CancellationToken cancellationToken = default);

        Task<Country?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        Task<Country?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<IEnumerable<Country>> GetAllAsync(CancellationToken cancellationToken = default);

        Task DeleteAsync(Country country, CancellationToken cancellationToken = default);
    }

    public interface ICityRepository
    {
        Task AddAsync(City city, CancellationToken cancellationToken = default);

        Task<PageResult<City>> GetAllAsync(Guid countryId, PageRequest pageRequest, CancellationToken cancellationToken = default);
    }
}
