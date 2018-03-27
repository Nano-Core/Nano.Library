using System;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nano.Models;
using Nano.Models.Criterias;
using Nano.Web.Controllers;

namespace Tests.Nano.Web.Controllers
{
    [TestClass]
    public class DefaultControllerTest : BaseControllerTest<DefaultController<DefaultEntity, DefaultQueryCriteria>>
    {
        [TestMethod]
        public void IndexTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DetailsViewTest()
        {
            this.Service
                .Setup(x => x.GetAsync<DefaultEntity, Guid>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DefaultEntity());

            var guid = Guid.NewGuid();
            var result = this.Controller.Details(guid).Result as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void DetailsOkObjectTest()
        {
            this.Service
                .Setup(x => x.GetAsync<DefaultEntity, Guid>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DefaultEntity());

            var guid = Guid.NewGuid();
            var result = this.Controller.Details(guid).Result as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void DetailsWhenNotFoundTest()
        {
            this.Service
                .Setup(x => x.GetAsync<DefaultEntity, Guid>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DefaultEntity)null);

            var guid = Guid.NewGuid();
            var result = this.Controller.Details(guid).Result as NotFoundResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
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
        public void EditConfirmsQueryTest()
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
        public void DeleteConfirmsQueryTest()
        {
            Assert.Inconclusive();
        }
    }
}