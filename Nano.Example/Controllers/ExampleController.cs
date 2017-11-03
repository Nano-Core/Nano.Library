using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Example.Controllers.Contracts;
using Nano.Example.Models;
using Nano.Services.Interfaces;

namespace Nano.Example.Controllers
{
    /// <inheritdoc />
    public class ExampleController : DefaultController<ExampleEntity, ExampleCriteria>
    {
        /// <inheritdoc />
        public ExampleController(ILogger<ExampleController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}