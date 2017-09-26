using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Globale.Models;
using Nano.Services.Interfaces;

namespace Nano.Services.Globale.Controllers
{
    /// <inheritdoc />
    public class CurrenciesController : BaseController<IService, Currency>
    {
        /// <inheritdoc />
        public CurrenciesController(ILogger<CurrenciesController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}