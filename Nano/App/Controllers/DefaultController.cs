using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Controllers.Criteria.Interfaces;
using Nano.App.Models;
using Nano.App.Services.Interfaces;

namespace Nano.App.Controllers
{
    /// <inheritdoc />
    public class DefaultController<TEntity, TCriteria> : BaseController<IService, TEntity, Guid, TCriteria>
        where TEntity : DefaultEntity
        where TCriteria : class, ICriteria
    {
        /// <inheritdoc />
        protected DefaultController(ILogger<Controller> logger, IService service)
            : base(logger, service)
        {

        }
    }
}