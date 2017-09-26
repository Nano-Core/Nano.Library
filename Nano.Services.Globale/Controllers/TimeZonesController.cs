using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Interfaces;
using TimeZone = Nano.Services.Globale.Models.TimeZone;

namespace Nano.Services.Globale.Controllers
{
    /// <inheritdoc />
    public class TimeZonesController : BaseController<IService, TimeZone>
    {
        /// <inheritdoc />
        public TimeZonesController(ILoggerFactory loggerFactory, IService service)
            : base(loggerFactory, service)
        {

        }
    }
}