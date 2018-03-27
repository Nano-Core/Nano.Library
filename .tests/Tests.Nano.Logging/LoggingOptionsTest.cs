using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Logging;

namespace Tests.Nano.Logging
{
    [TestClass]
    public class LoggingOptionsTest
    {
        [TestMethod]
        public void GetSectionNameTest()
        {
            Assert.AreEqual("Logging", LoggingOptions.SectionName);
        }

        [TestMethod]
        public void DefaultValuesTest()
        {
            Assert.Inconclusive();
        }
    }
}