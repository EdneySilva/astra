using Astra.Domain.Specifications;
using System.Linq.Expressions;

namespace Astra.Domain.Abstractions.Data
{
    public interface ICountryRepository
    {
        Task AddAsync(Country country, CancellationToken cancellationToken = default);

        Task UpdateAsync(Country country, CancellationToken cancellationToken = default);

        Task<Country?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        
        Task<Country?> FindAsync(ISpecification<Country> specification, CancellationToken cancellationToken = default);        

        Task<IEnumerable<Country>> GetAllAsync(ISpecification<Country> specification, CancellationToken cancellationToken = default);

        Task DeleteAsync(Country country, CancellationToken cancellationToken = default);
    }
}
