using System;
using System.Linq;
using System.Linq.Expressions;
using Rh.Specifiable.Expressions;

namespace Rh.Specifiable.Specifications
{
    public class AndSpecification<T> : CompositeSpecification<T>
    {
        public AndSpecification(params Specification<T>[] specifications)
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
                .Aggregate(firstSpecification, 
                    (current, specification) => current.And(specification.ToExpression()));
        }

        public override bool IsSatisfiedBy(T target)
        {
            return Specifications.All(s => s.IsSatisfiedBy(target));
        }
    }
}