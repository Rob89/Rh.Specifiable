using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rh.Specifiable.Queryable
{
    // Based on:
    // http://tomasp.net/blog/linq-expand.aspx
    // http://www.albahari.com/nutshell/linqkit.html
    public class VisitableQuery<T> : IOrderedQueryable<T>
    {
        private readonly VisitableQueryProvider<T> provider;

        internal IQueryable<T> InnerQuery { get; } 

        internal VisitableQuery(IQueryable<T> inner, params ExpressionVisitor[] visitors)
        {
            InnerQuery = inner;
            provider = new VisitableQueryProvider<T>(this, visitors);
        }

        System.Linq.Expressions.Expression IQueryable.Expression => InnerQuery.Expression;

        Type IQueryable.ElementType => typeof(T);

        IQueryProvider IQueryable.Provider => provider;

        public IEnumerator<T> GetEnumerator()
        {
            return InnerQuery.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerQuery.GetEnumerator();
        }

        public override string ToString()
        {
            return InnerQuery.ToString();
        }
    }
}