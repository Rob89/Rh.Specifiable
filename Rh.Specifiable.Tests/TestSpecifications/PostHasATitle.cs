using System;
using System.Linq.Expressions;
using Rh.Specifiable.Specifications;
using Rh.Specifiable.Tests.TestObjects;

namespace Rh.Specifiable.Tests.TestSpecifications
{
    public class PostHasATitle : ExpressionSpecification<TestPost>
    {
        protected override Expression<Func<TestPost, bool>> Expression { get; } =
            post => !string.IsNullOrEmpty(post.Title);
    }
}