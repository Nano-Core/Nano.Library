using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Logging;

namespace Tests.Nano.Security
{
    [TestClass]
    public class SecurityOptionsTest
    {
        [TestMethod]
        public void GetSectionNameTest()
        {
            Assert.AreEqual("Security", LoggingOptions.SectionName);
        }

        [TestMethod]
        public void DefaultValuesTest()
        {
            Assert.Inconclusive();
        }
    }
}