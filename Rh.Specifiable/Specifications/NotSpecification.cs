using System;
using System.Linq.Expressions;
using Rh.Specifiable.Expressions;

namespace Rh.Specifiable.Specifications
{
    public class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> specification;

        public NotSpecification(Specification<T> specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }
            this.specification = specification;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            return specification.ToExpression().Negate();
        }

        public override bool IsSatisfiedBy(T target)
        {
            return !specification.IsSatisfiedBy(target);
        }
    }
}