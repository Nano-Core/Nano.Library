using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Criterias;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class AuditController : DefaultControllerReadOnly<DefaultAuditEntry, AuditEntryQueryCriteria>
    {
        /// <inheritdoc />
        protected AuditController(ILogger logger, IRepository repository, IEventing eventing) 
            : base(logger, repository, eventing)
        {

        }
    }
}