using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Controllers.Contracts.Interfaces;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;

namespace Nano.App.Controllers
{
    /// <inheritdoc />
    public class DefaultController<TEntity, TCriteria> : BaseController<IService, TEntity, Guid, TCriteria>
        where TEntity : class, IEntityIdentity<Guid>, IEntityWritable
        where TCriteria : class, ICriteria, new()
    {
        /// <inheritdoc />
        protected DefaultController(ILogger<Controller> logger, IService service)
            : base(logger, service)
        {

        }
    }
}