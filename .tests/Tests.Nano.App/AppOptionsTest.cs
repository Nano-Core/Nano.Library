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
        public void ConstructorDefaultValuesTest()
        {
            var options = new AppOptions();

            Assert.AreEqual("Application", options.Name);
            Assert.IsNull(options.Description);
            Assert.IsNull(options.TermsOfService);
            Assert.AreEqual("UTC", options.DefaultTimeZone);
            Assert.AreEqual("1.0.0.0", options.Version);
            Assert.IsNotNull(options.Cultures);
            Assert.AreEqual("en-US", options.Cultures.Default);
            Assert.IsNotNull(options.Cultures.Supported);
        }
    }
}