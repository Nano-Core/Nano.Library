using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Eventing;

namespace Tests.Nano.Eventing
{
    [TestClass]
    public class EventingOptionsTest
    {
        [TestMethod]
        public void GetSectionNameTest()
        {
            Assert.AreEqual("Eventing", EventingOptions.SectionName);
        }

        [TestMethod]
        public void DefaultValuesTest()
        {
            Assert.Inconclusive();
        }
    }
}