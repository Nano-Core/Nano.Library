using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Controllers.Criterias.Interfaces;
using Nano.Models;
using Nano.Services.Interfaces;

namespace Nano.Controllers
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