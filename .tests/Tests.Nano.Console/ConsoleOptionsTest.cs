using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.App;
using Nano.Console;

namespace Tests.Nano.Console
{
    [TestClass]
    public class ConsoleOptionsTest
    {
        [TestMethod]
        public void GetSectionNameTest()
        {
            Assert.AreEqual("Console", ConsoleOptions.SectionName);
        }

        [TestMethod]
        public void ConstructorDefaultValuesTest()
        {
            var options = new AppOptions();

            Assert.IsNotNull(options);
        }
    }
}