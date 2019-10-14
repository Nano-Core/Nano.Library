using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Models.Attributes;

namespace Tests.Nano.Models.Attributes
{
    [TestClass]
    public class IncludeAttributeTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var includeAttribute = new IncludeAttribute();

            Assert.IsNotNull(includeAttribute);
        }
    }
}