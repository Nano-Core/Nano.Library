using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.App;

namespace Tests.Nano.Web
{
    [TestClass]
    public class WebOptionsTest
    {
        [TestMethod]
        public void GetSectionNameTest()
        {
            Assert.AreEqual("Web", AppOptions.SectionName);
        }

        [TestMethod]
        public void DefaultValuesTest()
        {
            Assert.Inconclusive();
        }
    }
}