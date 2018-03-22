using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Nano.Web.Controllers
{
    [TestClass]
    public class DefaultControllerTest : BaseControllerTest<TestController>
    {
        [TestMethod]
        public void IndexTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DetailsTest()
        {
            var result = this.Controller.Details(Guid.NewGuid()).Result as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void DetailssTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void QueryTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void QueryPostTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CreateTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CreateConfirmTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CreateConfirmsTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void EditTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void EditConfirmTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void EditConfirmsTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void EditConfirmQueryTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeleteTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeleteConfirmTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeleteConfirmsTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeleteConfirmQueryTest()
        {
            Assert.Inconclusive();
        }
    }
}