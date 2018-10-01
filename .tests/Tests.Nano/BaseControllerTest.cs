using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;
using Nano.Web.Controllers;
using Nano.Web.Hosting;

namespace Tests.Nano
{
    public abstract class BaseControllerTest<TController> : BaseTest
        where TController : BaseController<IRepository>
    {
        protected virtual TController Controller { get; set; }
        protected virtual Mock<ILogger> Logger { get; set; } = new Mock<ILogger>();
        protected virtual Mock<IRepository> Service { get; set; } = new Mock<IRepository>();
        protected virtual Mock<IEventing> Eventing { get; set; } = new Mock<IEventing>();

        [TestCleanup]
        public override void Cleanup()
        {
            this.Logger = null;
            this.Service = null;
            this.Eventing = null;
            this.Controller = null;
        }

        [TestInitialize]
        public override void Initialize()
        {
            var controllerMock = new Mock<TController>(this.Logger.Object, this.Service.Object, this.Eventing.Object)
            {
                CallBase = true
            };

            var httpContext = new Mock<HttpContext>();
            var httpRequest = new Mock<HttpRequest>();
            var httpResponse = new Mock<HttpResponse>();

            httpRequest
                .SetupGet(x => x.Headers)
                .Returns(new HeaderDictionary
                {
                    { "Accept", HttpContentType.JSON }
                });

            httpContext
                .SetupGet(x => x.Request)
                .Returns(httpRequest.Object);

            httpContext
                .SetupGet(x => x.Response)
                .Returns(httpResponse.Object);

            this.Controller = controllerMock.Object;
            this.Controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };
        }
    }
}