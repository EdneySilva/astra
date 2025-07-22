using Astra.Domain;
using Astra.Domain.Abstractions;
using Astra.Domain.Abstractions.Data;
using Astra.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public Task<City?> FindAync(ISpecification<City> specification, CancellationToken cancellationToken = default)
        {
            return _cities.FirstOrDefaultAsync(specification.ToExpression(), cancellationToken);
        }
        
        public Task<City?> FindCityOnCountryAync(int countryId, City city, CancellationToken cancellationToken = default)
        {
            return _cities.FirstOrDefaultAsync(f => f.Country.Id == countryId && f.Name == city.Name && f.Province == city.Province, cancellationToken);
        }

        public async Task<PageResult<City>> GetAllAsync(ISpecification<City> specification, PageRequest pageRequest, CancellationToken cancellationToken = default)
        {
            var query = _cities
                .Where(specification.ToExpression());
            var pageSize = pageRequest.PageSize;
            var page = pageRequest.Page;
            var result = await query
                .Select(c => new { Total = 1, City = new City
                {
                    Country = new Country
                    {
                        Code = c.Country.Code,
                        Name = c.Country.Name,
                        Deleted = c.Country.Deleted,
                        Id = c.Country.Id
                    },
                    Name = c.Name,
                    Province = c.Province,
                    Id = c.Id
                }})
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Items = g
                        .OrderBy(x => x.City.Id)
                        .Skip((page -1) * pageSize)
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

        public Task<City?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _cities.Include(c => c.Country).FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public Task DeleteCity(City city, CancellationToken cancellationToken = default)
        {
            city.Deleted = true;
            _context.Update(city);
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
