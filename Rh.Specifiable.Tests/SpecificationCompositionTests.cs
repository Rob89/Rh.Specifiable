using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rh.Specifiable.Specifications;
using Rh.Specifiable.Tests.TestObjects;
using Rh.Specifiable.Tests.TestSpecifications;

namespace Rh.Specifiable.Tests
{
    [TestClass]
    public class SpecificationCompositionTests
    {
        private readonly IQueryable<TestPost> TestData = new List<TestPost>
        {
            new TestPost {Content = "Hello world", Id = 1, Title = ""},
            new TestPost {Content = "", Id = 2, Title = "Hello world"},
            new TestPost {Content = "Hello world", Id = 3, Title = "Hello world"}
        }.AsQueryable();

        [TestMethod]
        public void ASpecificationShouldBeImplicitlyConvertableToAnExpression()
        {
            var hasTitleSpecification = new PostHasATitle();
            Expression<Func<TestPost, bool>> result = null;
            Exception caughtException = null;

            try
            {
                result = hasTitleSpecification;
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.IsNull(caughtException);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NegatingASpecificationShouldReturnANotSpecification()
        {
            var hasTitleSpecification = new PostHasATitle();
            var result = hasTitleSpecification.Negate();

            Assert.IsInstanceOfType(result, typeof (NotSpecification<TestPost>));
        }

        [TestMethod]
        public void UnaryNegationOfASpecificationShouldReturnANotSpecification()
        {
            var hasTitleSpecification = new PostHasATitle();
            var result = !hasTitleSpecification;

            Assert.IsInstanceOfType(result, typeof (NotSpecification<TestPost>));
        }

        [TestMethod]
        public void AndingTwoSpecificationsShouldReturnAnAndSpecification()
        {
            var hasTitleSpecification = new PostHasATitle();
            var hasContentSpecification = new PostHasContent();

            var result = hasTitleSpecification.And(hasContentSpecification);

            Assert.IsInstanceOfType(result, typeof (AndSpecification<TestPost>));
        }

        [TestMethod]
        public void OringTwoSpecificationsShouldReturnAnAndSpecification()
        {
            var hasTitleSpecification = new PostHasATitle();
            var hasContentSpecification = new PostHasContent();

            var result = hasTitleSpecification.Or(hasContentSpecification);

            Assert.IsInstanceOfType(result, typeof (OrSpecification<TestPost>));
        }

        [TestMethod]
        public void AnAndSpecificationShouldApplyBothSpecifications()
        {
            var hasTitleSpecification = new PostHasATitle();
            var hasContentSpecification = new PostHasContent();

            var hasContentAndTitleSpecification = hasTitleSpecification.And(hasContentSpecification);

            var result = TestData.Where(hasContentAndTitleSpecification).Single();

            Assert.AreEqual(result.Id, 3);
        }

        [TestMethod]
        public void AnOrSpecificationShouldApplyBothSpecifications()
        {
            var hasTitleSpecification = new PostHasATitle();
            var hasContentSpecification = new PostHasContent();

            var hasContentOrTitleSpecification = hasTitleSpecification.Or(hasContentSpecification);

            var result = TestData.Where(hasContentOrTitleSpecification).ToList();

            Assert.AreEqual(result.Count, TestData.Count());
        }
    }
}