using System;
using System.Linq.Expressions;

namespace Rh.Specifiable.Specifications
{
    public abstract class Specification<T>
    {
        public abstract Expression<Func<T, bool>> ToExpression();

        public abstract bool IsSatisfiedBy(T target);

        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }
            return specification.ToExpression();
        }

        public Specification<T> And(Specification<T> other)
        {
            ValidateForCompose(other);
            return new AndSpecification<T>(this, other);
        }

        public Specification<T> Or(Specification<T> other)
        {
            ValidateForCompose(other);
            return new OrSpecification<T>(this, other);
        }

        public Specification<T> Negate()
        {
            return new NotSpecification<T>(this);
        }

        public static Specification<T> operator ! (Specification<T> specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }
            return specification.Negate();
        }

        private void ValidateForCompose(Specification<T> other)
        {
            if (ToExpression() == null)
            {
                throw new InvalidOperationException(
                    "Cannot compose an empty specification with another specification.");
            }
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
        }
    }
}