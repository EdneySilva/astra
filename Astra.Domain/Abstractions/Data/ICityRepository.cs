using Astra.Domain.Specifications;

namespace Astra.Domain.Abstractions.Data
{
    public interface ICityRepository
    {
        Task AddAsync(City city, CancellationToken cancellationToken = default);

        Task<City?> FindAync(ISpecification<City> specification, CancellationToken cancellationToken = default);

        Task<City?> FindByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<PageResult<City>> GetAllAsync(ISpecification<City> specification, PageRequest pageRequest, CancellationToken cancellationToken = default);

        Task DeleteCity(City city, CancellationToken cancellationToken = default);
    }
}
