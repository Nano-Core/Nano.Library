using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Globale.Models;
using Nano.Services.Interfaces;

namespace Nano.Services.Globale.Controllers
{
    /// <inheritdoc />
    public class CountriesController : BaseController<IService, Country>
    {
        /// <inheritdoc />
        public CountriesController(ILogger<CountriesController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}

