using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Globale.Models;
using Nano.Services.Interfaces;

namespace Nano.Services.Globale.Controllers
{
    /// <inheritdoc />
    public class CitiesController : DefaultController<City>
    {
        /// <inheritdoc />
        public CitiesController(ILogger<CitiesController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}