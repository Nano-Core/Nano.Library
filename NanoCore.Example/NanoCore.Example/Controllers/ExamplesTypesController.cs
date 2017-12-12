using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;
using Nano.Web.Controllers;
using NanoCore.Example.Models;
using NanoCore.Example.Models.Criterias;

namespace NanoCore.Example.Controllers
{
    /// <inheritdoc />
    public class ExamplesTypesController : DefaultController<ExampleEntityTypes, ExampleTypesQueryCriteria>
    {
        /// <inheritdoc />
        public ExamplesTypesController(ILogger logger, IService service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }
    }
}