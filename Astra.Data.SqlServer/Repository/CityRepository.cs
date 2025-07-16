using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Astra.Data.SqlServer.Repository
{
    public class CityRepository : ICityRepository
    {
        DbSet<City> _cities;
        ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
            _cities = applicationDbContext.Set<City>();
        }

        public async Task AddAsync(City city, CancellationToken cancellationToken = default)
        {
            await _cities.AddAsync(city, cancellationToken);
            await _context.SaveChangesAsync();
        }

        public async Task<PageResult<City>> GetAllAsync(Guid countryId, PageRequest pageRequest, CancellationToken cancellationToken = default)
        {
            var query = _cities
                .Where(c => c.Country.Id == countryId);
            var pageSize = pageRequest.PageSize;
            var page = pageRequest.Page;
            var result = await query
                .Select(c => new { Total = 1, City = c }) // marca os itens
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Items = g
                        .OrderBy(x => x.City.Id)
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Select(x => x.City)
                })
                .FirstOrDefaultAsync(cancellationToken);
            return new PageResult<City>
            {
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize,
                Items = result?.Items ?? new List<City>(),
                TotalItemsFound = result?.Total ?? 0
            };
        }
    }
}
