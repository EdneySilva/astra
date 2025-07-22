using Astra.Domain;
using Astra.Domain.Specifications;
using System.Linq.Expressions;

namespace Astra.Manager.Data.City
{
    internal class CityQueries : ISpecification<Domain.City>
    {
        Expression<Func<Domain.City, bool>> _expression;

        CityQueries(Expression<Func<Domain.City, bool>> expression)
        {
            _expression = expression;
        }

        public static ISpecification<Domain.City> AllOnCountry(int countryId)
        {
            return new CityQueries((f) => f.Country.Id == countryId);
        }

        public static ISpecification<Domain.City> CityOnCountry(int countryId, Domain.City city)
        {
            return new CityQueries((f) => f.Country.Id == countryId && f.Name == city.Name && f.Province == city.Province);
        }

        public Expression<Func<Domain.City, bool>> ToExpression()
        {
            return _expression;
        }
    }
}
