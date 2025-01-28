using Microsoft.EntityFrameworkCore;
using System.Linq;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Specifications;

namespace Teamo.Infrastructure.Data
{
    public static class UserSpecificationEvaluator
    {
        public static IQueryable<User> GetQuery(IQueryable<User> inputQuery, ISpecification<User> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
