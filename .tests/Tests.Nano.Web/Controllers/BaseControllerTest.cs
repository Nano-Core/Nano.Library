using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;

namespace Tests.Nano.Web.Controllers
{
    public abstract class BaseControllerTest<TController> : BaseTest
        where TController : class
    {
        protected virtual TController Controller { get; set; }
        protected virtual Mock<ILogger> Logger { get; set; } = new Mock<ILogger>();
        protected virtual Mock<IService> Service { get; set; } = new Mock<IService>();
        protected virtual Mock<IEventing> Eventing { get; set; } = new Mock<IEventing>();

        [TestCleanup]
        public void Cleanup()
        {
            this.Controller = null;
        }

        [TestInitialize]
        public void Initialize()
        {
            var mock = new Mock<TController>(this.Logger.Object, this.Service.Object, this.Eventing.Object)
            {
                CallBase = true
            };

            this.Controller = mock.Object;
        }
    }
}