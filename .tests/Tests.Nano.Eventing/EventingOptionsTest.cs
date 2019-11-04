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
            var options = new EventingOptions();

            Assert.IsNull(options.Host);
            Assert.AreEqual("/", options.VHost);
            Assert.AreEqual(string.Empty, options.Username);
            Assert.AreEqual(string.Empty, options.Password);
            Assert.AreEqual(5672, options.Port);
            Assert.AreEqual(30, options.Timeout);
            Assert.AreEqual(false, options.UseSsl);
            Assert.AreEqual(0, options.Heartbeat);
            Assert.AreEqual(true, options.UseHealthCheck);
            Assert.AreEqual($"amqp://{options.Username}:{options.Password}@{options.Host}:{options.Port}{options.VHost}", options.ConnectionString);
        }

        [TestMethod]
        public void PropertyConnectingStringTest()
        {
            var options = new EventingOptions
            {
                Host = "host", 
                VHost = "vhost", 
                Username = "username", 
                Password = "password"
            };

            Assert.AreEqual($"amqp://{options.Username}:{options.Password}@{options.Host}:{options.Port}{options.VHost}", options.ConnectionString);
        }
    }
}