using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rh.Specifiable.Expressions;
using Rh.Specifiable.Tests.TestObjects;
using Rh.Specifiable.Tests.TestSpecifications;

namespace Rh.Specifiable.Tests
{
    [TestClass]
    public class SpecificationExpanderTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var postHasATitle = new PostHasATitle();
            var expander = new SpecificationExpander();
            Expression<Func<IEnumerable<TestPost>, bool>> someExpression =
                posts => posts.AsQueryable().Any(postHasATitle);
            Expression<Func<IEnumerable<TestPost>, bool>> expectedExpression =
                posts => posts.AsQueryable().Any(post => !string.IsNullOrEmpty(post.Title));

            var resultingExpression = someExpression.VisitWith(expander);

            Assert.AreEqual(resultingExpression.ToString(), expectedExpression.ToString());
        }
    }
}