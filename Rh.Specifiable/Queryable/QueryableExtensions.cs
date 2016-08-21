using System.Linq;
using Rh.Specifiable.Expressions;

namespace Rh.Specifiable.Queryable
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> AsSpecifiable<T>(this IQueryable<T> query)
        {
            var visitableQuery = query as VisitableQuery<T>;
            return visitableQuery ?? new VisitableQuery<T>(query, new SpecificationExpander());
        }
    }
}

