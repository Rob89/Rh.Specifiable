using System;
using System.Linq.Expressions;

namespace Rh.Specifiable.Specifications
{
    public abstract class ExpressionSpecification<T> : Specification<T>
    {
        protected abstract Expression<Func<T, bool>> Expression { get; }

        public override Expression<Func<T, bool>> ToExpression()
        {
            return Expression;
        }

        public override bool IsSatisfiedBy(T target)
        {
            return Expression.Compile()(target);
        }
    }
}