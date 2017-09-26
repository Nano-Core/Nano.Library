using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Globale.Models;
using Nano.Services.Interfaces;

namespace Nano.Services.Globale.Controllers
{
    /// <inheritdoc />
    public class LanguagesController : BaseController<IService, Language>
    {
        /// <inheritdoc />
        public LanguagesController(ILoggerFactory loggerFactory, IService service)
            : base(loggerFactory, service)
        {

        }
    }
}