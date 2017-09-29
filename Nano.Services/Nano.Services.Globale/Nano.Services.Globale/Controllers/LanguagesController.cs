using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Globale.Models;
using Nano.Services.Interfaces;

namespace Nano.Services.Globale.Controllers
{
    /// <inheritdoc />
    public class LanguagesController : DefaultController<Language>
    {
        /// <inheritdoc />
        public LanguagesController(ILogger<LanguagesController> logger, IService service)
            : base(logger, service)
        {

        }
    }
}