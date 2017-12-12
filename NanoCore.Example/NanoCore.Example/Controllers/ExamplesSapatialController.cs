using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;
using Nano.Web.Controllers;
using NanoCore.Example.Models;
using NanoCore.Example.Models.Criterias;

namespace NanoCore.Example.Controllers
{
    /// <inheritdoc />
    public class ExamplesSapatialController : DefaultController<ExampleEntitySpatial, ExampleSpatialQueryCriteria>
    {
        /// <inheritdoc />
        public ExamplesSapatialController(ILogger logger, IService service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }
    }
}