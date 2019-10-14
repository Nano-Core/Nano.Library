using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Logging;
using Serilog.Events;

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
            var options = new LoggingOptions();

            Assert.AreEqual(LogEventLevel.Information, options.LogLevel);
            Assert.IsNotNull(options.LogLevelOverrides);
        }
    }
}