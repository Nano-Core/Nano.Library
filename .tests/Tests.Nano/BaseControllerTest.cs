using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nano.Eventing.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Web.Controllers;
using Nano.Web.Hosting;

namespace Tests.Nano
{
    /// <inheritdoc />
    public abstract class BaseControllerTest<TController> : BaseTest
        where TController : BaseController<IRepository>
    {
        /// <summary>
        /// Controller.
        /// THe initialized controller of type <typeparamref name="TController"/>.
        /// All dependencies has been mocked and resolved, and is ready for invoking actions.
        /// </summary>
        protected virtual TController Controller { get; set; }

        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual Mock<ILogger> MockLogger { get; set; } = new Mock<ILogger>();

        /// <summary>
        /// Eventing.
        /// </summary>
        protected virtual Mock<IEventing> MockEventing { get; set; } = new Mock<IEventing>();

        /// <summary>
        /// Repository.
        /// </summary>
        protected virtual Mock<IRepository> MockRepository { get; set; } = new Mock<IRepository>();

        /// <summary>
        /// Controller.
        /// </summary>
        protected virtual Mock<TController> MockController { get; set; }

        /// <inheritdoc />
        protected BaseControllerTest()
        {
            this.MockController = new Mock<TController>(this.MockLogger.Object, this.MockRepository.Object, this.MockEventing.Object)
            {
                CallBase = true
            };
        }

        /// <inheritdoc />
        [TestCleanup]
        public override void Cleanup()
        {
            this.MockEventing.Object?.Dispose();
            this.MockRepository.Object?.Dispose();
            this.Controller.Dispose();

            this.MockLogger = null;
            this.MockEventing = null;
            this.MockRepository = null;
            this.MockController = null;
        }

        /// <inheritdoc />
        [TestInitialize]
        public override void Initialize()
        {
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

            this.Controller = this.MockController.Object;
            this.Controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };
        }
    }
}