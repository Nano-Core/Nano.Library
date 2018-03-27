using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.App;

namespace Tests.Nano.App
{
    [TestClass]
    public class AppOptionsTest
    {
        [TestMethod]
        public void GetSectionNameTest()
        {
            Assert.AreEqual("App", AppOptions.SectionName);
        }

        [TestMethod]
        public void DefaultValuesTest()
        {
            Assert.Inconclusive();
        }
    }
}