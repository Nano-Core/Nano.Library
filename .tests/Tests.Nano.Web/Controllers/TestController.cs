using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;
using Nano.Web.Controllers;

namespace Tests.Nano.Web.Controllers
{
    /// <inheritdoc />
    public class TestController : DefaultController<TestEntity, TestQueryCriteria>
    {
        /// <inheritdoc />
        public TestController(ILogger logger, IService service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }
    }
}