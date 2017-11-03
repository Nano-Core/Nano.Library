using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Controllers.Criterias.Interfaces;
using Nano.Models;
using Nano.Services.Interfaces;

namespace Nano.Controllers
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