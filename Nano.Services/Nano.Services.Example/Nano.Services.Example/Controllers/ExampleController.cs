using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Example.Models;
using Nano.Services.Interfaces;

namespace Nano.Services.Example.Controllers
{
    /// <inheritdoc />
    public class ExampleController : BaseController<IService, ExampleEntity>
    {
        /// <inheritdoc />
        public ExampleController(ILogger<ExampleController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}