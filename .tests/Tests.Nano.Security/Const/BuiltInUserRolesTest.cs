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
            Assert.AreEqual("guest", BuiltInUserRoles.GUEST);
        }

        [TestMethod]
        public void ReaderTest()
        {
            Assert.AreEqual("reader", BuiltInUserRoles.READER);
        }

        [TestMethod]
        public void Writerest()
        {
            Assert.AreEqual("writer", BuiltInUserRoles.WRITER);
        }

        [TestMethod]
        public void ServiceTest()
        {
            Assert.AreEqual("service", BuiltInUserRoles.SERVICE);
        }

        [TestMethod]
        public void AdministratorTest()
        {
            Assert.AreEqual("administrator", BuiltInUserRoles.ADMINISTRATOR);
        }
    }
}