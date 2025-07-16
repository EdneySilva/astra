using Astra.Domain;
using Astra.Domain.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Astra.Data.SqlServer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }

}

namespace Astra.Data.Repository
{

    public class CountryRepository : ICountryRepository
    {
        public Task AddAsync(Country country, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Country country, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Country> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Country> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Country>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
