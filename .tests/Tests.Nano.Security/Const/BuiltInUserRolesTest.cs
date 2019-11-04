using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Security.Const;

namespace Tests.Nano.Security.Const
{
    [TestClass]
    public class BuiltInUserRolesTest
    {
        [TestMethod]
        public void GuestTest()
        {
            Assert.AreEqual("guest", BuiltInUserRoles.Guest);
        }

        [TestMethod]
        public void ReaderTest()
        {
            Assert.AreEqual("reader", BuiltInUserRoles.Reader);
        }

        [TestMethod]
        public void Writerest()
        {
            Assert.AreEqual("writer", BuiltInUserRoles.Writer);
        }

        [TestMethod]
        public void ServiceTest()
        {
            Assert.AreEqual("service", BuiltInUserRoles.Service);
        }

        [TestMethod]
        public void AdministratorTest()
        {
            Assert.AreEqual("administrator", BuiltInUserRoles.Administrator);
        }
    }
}