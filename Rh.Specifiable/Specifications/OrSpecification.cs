using System;
using System.Linq;
using System.Linq.Expressions;
using Rh.Specifiable.Expression;

namespace Rh.Specifiable.Specifications
{
    public class OrSpecification<T> : CompositeSpecification<T>
    {
        public OrSpecification(params Specification<T>[] specifications)
            : base(specifications)
        {
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var firstSpecification = Specifications.First().ToExpression();
            if (Specifications.Length == 1)
            {
                return firstSpecification;
            }
            return Specifications.Skip(1)
                .Aggregate(firstSpecification, (current, specification) => current.Or(specification.ToExpression()));
        }

        public override bool IsSatisfiedBy(T target)
        {
            return Specifications.Any(s => s.IsSatisfiedBy(target));
        }
    }
}