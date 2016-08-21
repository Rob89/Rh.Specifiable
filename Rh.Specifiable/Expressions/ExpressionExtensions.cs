using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rh.Specifiable.Expressions
{
    // This was inspired by:
    // https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
    // https://github.com/juanplopes/simple/blob/master/src/Simple/Expressions/PredicateBuilder.cs
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return True<T>(System.Linq.Expressions.Expression.Parameter(typeof(T), "f"));
        }

        public static Expression<Func<T, bool>> True<T>(ParameterExpression param)
        {
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.Constant(true), param);
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return False<T>(System.Linq.Expressions.Expression.Parameter(typeof(T), "f"));
        }
        public static Expression<Func<T, bool>> False<T>(ParameterExpression param)
        {
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.Constant(false), param);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var invokedExpr = System.Linq.Expressions.Expression.Invoke(second, first.Parameters);
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>
                  (System.Linq.Expressions.Expression.AndAlso(first.Body, invokedExpr), first.Parameters).ExpandInvocations();
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var invokedExpr = System.Linq.Expressions.Expression.Invoke(second, first.Parameters);
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>
                  (System.Linq.Expressions.Expression.OrElse(first.Body, invokedExpr), first.Parameters).ExpandInvocations();
        }

        public static Expression<Func<T, bool>> Negate<T>(this Expression<Func<T, bool>> expr)
        {
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.Not(expr.Body), expr.Parameters).ExpandInvocations();
        }

        private static Expression<T> ExpandInvocations<T>(this Expression<T> expr)
        {
            return expr.VisitWith(new InvocationExpander());
        }

        public static Expression<T> VisitWith<T>(this Expression<T> expression, params ExpressionVisitor[] visitors)
        {
            return (Expression<T>)((System.Linq.Expressions.Expression)expression).VisitWith(visitors);
        }

        public static System.Linq.Expressions.Expression VisitWith(this System.Linq.Expressions.Expression expression, params ExpressionVisitor[] visitors)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            return visitors == null
                ? expression 
                : visitors.Aggregate(expression, (current, visitor) => visitor.Visit(current));
        }
    }
}
