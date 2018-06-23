using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Criterias;
using Nano.Services.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class AuditController : DefaultControllerReadOnly<DefaultAuditEntry, AuditEntryQueryCriteria>
    {
        /// <inheritdoc />
        protected AuditController(ILogger logger, IService service, IEventing eventing) 
            : base(logger, service, eventing)
        {

        }
    }
}