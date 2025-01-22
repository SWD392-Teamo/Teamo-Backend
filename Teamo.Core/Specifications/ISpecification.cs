using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Teamo.Core.Specifications
{
    /// <summary>
    /// Custom specifications for entities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria {  get; }
        List<Expression<Func<T, object>>> Includes { get;}
        List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> CustomIncludes { get;}
        Expression<Func<T, object>> OrderBy { get;}
        Expression<Func<T, object>> OrderByDescending { get;}
        int Take {  get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
