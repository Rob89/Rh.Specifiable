using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rh.Specifiable.Queryable;
using Rh.Specifiable.Tests.TestObjects;

namespace Rh.Specifiable.Tests
{
    [TestClass]
    public class VisitableQueryTests
    {
        [TestMethod]
        public void CallingAsSpecifiableShouldReturnAVisitableQuery()
        {
            var queryable = new List<TestPost>().AsQueryable();

            var result = queryable.AsSpecifiable();

            Assert.IsInstanceOfType(result, typeof (VisitableQuery<TestPost>));
        }
    }
}