using Astra.Domain.Specifications;
using System.Linq.Expressions;

namespace Astra.Manager.Data.Country
{

    internal class CountryQueries : ISpecification<Domain.Country>
    {
        Expression<Func<Domain.Country, bool>> _expression;

        CountryQueries(Expression<Func<Domain.Country, bool>> expression)
        {
            _expression = expression;
        }

        public static ISpecification<Domain.Country> WithName(string name)
        {
            return new CountryQueries(f => f.Name == name);
        }
        
        public static ISpecification<Domain.Country> All()
        {
            return new CountryQueries(f => true);
        }

        public Expression<Func<Domain.Country, bool>> ToExpression()
        {
            return _expression;
        }
    }
}
