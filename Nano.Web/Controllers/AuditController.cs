using Microsoft.Extensions.Logging;
using Nano.Data.Models;
using Nano.Data.Models.Criterias;
using Nano.Eventing.Interfaces;
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