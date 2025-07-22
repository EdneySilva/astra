using Astra.Domain;
using HotChocolate;
using HotChocolate.Types;

namespace Astra.Data.GraphQl.Queries
{
    public class Query
    {
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Country> GetCountries([Service] IQueryable<Country> repository)
        {
            return repository;
        }

        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<City> GetCities([Service] IQueryable<City> repository)
        {
            return repository;
        }
    }

}
