using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Interfaces;
using TimeZone = Nano.Services.Globale.Models.TimeZone;

namespace Nano.Services.Globale.Controllers
{
    /// <inheritdoc />
    public class TimeZonesController : DefaultController<TimeZone>
    {
        /// <inheritdoc />
        public TimeZonesController(ILogger<TimeZonesController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}