using Microsoft.Extensions.Logging;
using Nano.App.Controllers;
using Nano.App.Services.Interfaces;
using Nano.Example.Models;

namespace Nano.Example.Controllers
{
    /// <inheritdoc />
    public class ExampleController : DefaultController<ExampleEntity>
    {
        /// <inheritdoc />
        public ExampleController(ILogger<ExampleController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}