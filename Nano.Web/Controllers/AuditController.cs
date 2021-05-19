using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Nano.Data.Models;
using Nano.Data.Models.Criterias;
using Nano.Eventing;
using Nano.Eventing.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    public class AuditController : DefaultControllerReadOnly<DefaultAuditEntry, AuditEntryQueryCriteria>
    {
        /// <inheritdoc />
        public AuditController(ILogger logger, IRepository repository)
            : this(logger, repository, new NullEventing())
        {

        }

        /// <inheritdoc />
        public AuditController(ILogger logger, IRepository repository, IEventing eventing) 
            : base(logger, repository, eventing)
        {

        }
    }
}