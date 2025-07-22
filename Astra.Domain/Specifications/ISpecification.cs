using System.Linq.Expressions;

namespace Astra.Domain.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> ToExpression();
    }
}
