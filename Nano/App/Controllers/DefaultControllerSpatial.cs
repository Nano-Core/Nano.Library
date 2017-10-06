using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Controllers.Contracts.Interfaces;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;

namespace Nano.App.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerSpatial<TEntity, TCriteria> : BaseControllerSpatial<IServiceSpatial, TEntity, Guid, TCriteria>
        where TEntity : class, IEntityIdentity<Guid>, IEntitySpatial, IEntityWritable
        where TCriteria : class, ICriteriaSpatial, new()
    {
        /// <inheritdoc />
        protected DefaultControllerSpatial(ILogger<Controller> logger, IServiceSpatial service)
            : base(logger, service)
        {

        }
    }
}