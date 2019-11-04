using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Security.Const;

namespace Tests.Nano.Security.Const
{
    [TestClass]
    public class ClaimTypesExtendedTest
    {
        [TestMethod]
        public void AppIdTest()
        {
            Assert.AreEqual("appid", ClaimTypesExtended.AppId);
        }
    }
}