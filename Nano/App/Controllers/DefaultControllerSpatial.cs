using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Controllers.Criteria.Interfaces;
using Nano.App.Models;
using Nano.App.Services.Interfaces;

namespace Nano.App.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerSpatial<TEntity, TCriteria> : BaseControllerSpatial<IServiceSpatial, TEntity, Guid, TCriteria>
        where TEntity : DefaultEntitySpatial
        where TCriteria : class, ICriteriaSpatial
    {
        /// <inheritdoc />
        protected DefaultControllerSpatial(ILogger<Controller> logger, IServiceSpatial service)
            : base(logger, service)
        {

        }
    }
}