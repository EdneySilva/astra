using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using Astra.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Astra.Data.SqlServer.Repository
{
    public class CountryRepository : ICountryRepository
    {
        DbSet<Country> _countries;
        ApplicationDbContext _context;

        public CountryRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
            _countries = applicationDbContext.Set<Country>();
        }

        public async Task AddAsync(Country country, CancellationToken cancellationToken = default)
        {
            await _countries.AddAsync(country, cancellationToken);
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(Country country, CancellationToken cancellationToken = default)
        {
            _countries.Update(country);
            return _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Country country, CancellationToken cancellationToken = default)
        {
            _countries.Remove(country);
            await _context.SaveChangesAsync();
        }

        public async Task<Country?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entry = await _countries.FindAsync(id, cancellationToken);
            return entry;
        }

        public Task<Country?> FindAsync(ISpecification<Country> specification, CancellationToken cancellationToken = default)
        {
            return _countries.AsQueryable()                
                .FirstOrDefaultAsync(specification.ToExpression(), cancellationToken);
        }

        public Task<IEnumerable<Country>> GetAllAsync(ISpecification<Country> specification, CancellationToken cancellationToken = default)
        {
            return _countries.AsQueryable().Where(specification.ToExpression())
                .ToListAsync(cancellationToken).ContinueWith(task =>
            {
                return task.Result.AsEnumerable();
            });
        }
    }
}
