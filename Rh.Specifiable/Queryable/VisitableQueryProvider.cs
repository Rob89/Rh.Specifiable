using System.Linq;
using System.Linq.Expressions;
using Rh.Specifiable.Expressions;

namespace Rh.Specifiable.Queryable
{
    // Based on:
    // http://tomasp.net/blog/linq-expand.aspx
    // http://www.albahari.com/nutshell/linqkit.html
    public class VisitableQueryProvider<T> : IQueryProvider
    {
        private readonly ExpressionVisitor[] interceptors;

        private readonly VisitableQuery<T> query;

        internal VisitableQueryProvider(
            VisitableQuery<T> query,
            ExpressionVisitor[] interceptors)
        {
            this.query = query;
            this.interceptors = interceptors;
        }

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return
                new VisitableQuery<TElement>(
                    query.InnerQuery.Provider.CreateQuery<TElement>(
                        expression.VisitWith(interceptors)));
        }

        IQueryable IQueryProvider.CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return query.InnerQuery.Provider.CreateQuery(expression.VisitWith(interceptors));
        }

        TResult IQueryProvider.Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            return
                query.InnerQuery.Provider.Execute<TResult>(expression.VisitWith(interceptors));
        }

        object IQueryProvider.Execute(System.Linq.Expressions.Expression expression)
        {
            return query.InnerQuery.Provider.Execute(expression.VisitWith(interceptors));
        }
    }
}