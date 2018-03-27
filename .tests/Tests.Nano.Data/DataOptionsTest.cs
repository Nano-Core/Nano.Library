using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Data;

namespace Tests.Nano.Data
{
    [TestClass]
    public class DataOptionsTest
    {
        [TestMethod]
        public void GetSectionNameTest()
        {
            Assert.AreEqual("Data", DataOptions.SectionName);
        }

        [TestMethod]
        public void DefaultValuesTest()
        {
            Assert.Inconclusive();
        }
    }
}