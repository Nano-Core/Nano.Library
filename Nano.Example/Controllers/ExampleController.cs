using Microsoft.Extensions.Logging;
using Nano.App.Controllers;
using Nano.App.Services.Interfaces;
using Nano.Example.Controllers.Contracts;
using Nano.Example.Models;

namespace Nano.Example.Controllers
{
    /// <inheritdoc />
    public class ExampleController : DefaultController<ExampleEntity, ExampleQuery>
    {
        /// <inheritdoc />
        public ExampleController(ILogger<ExampleController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}