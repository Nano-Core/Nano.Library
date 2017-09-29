using Microsoft.Extensions.Logging;
using Nano.Controllers;
using Nano.Services.Globale.Models;
using Nano.Services.Interfaces;

namespace Nano.Services.Globale.Controllers
{
    /// <inheritdoc />
    public class AddressController : DefaultControllerSpatial<Address>
    {
        /// <inheritdoc />
        public AddressController(ILogger<AddressController> logger, IServiceSpatial service)
            : base(logger, service)
        {

        }
    }
}