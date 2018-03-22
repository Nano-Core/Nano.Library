using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Web.Controllers.Extensions;

namespace Tests.Nano.Web.Controllers.Extensions
{
    [TestClass]
    public class HttpContentTypeTest
    {
        [TestMethod]
        public void HttpContentTextHtmlTest()
        {
            Assert.AreEqual("text/html", HttpContentType.HTML);
        }

        [TestMethod]
        public void HttpContentTypeTextPlainTest()
        {
            Assert.AreEqual("text/plain", HttpContentType.TEXT);
        }

        [TestMethod]
        public void HttpContentTypeWhenJsonTest()
        {
            Assert.AreEqual("application/json", HttpContentType.JSON);
        }

        [TestMethod]
        public void HttpContentTypeXmlTest()
        {
            Assert.AreEqual("application/xml", HttpContentType.XML);
        }
    }
}