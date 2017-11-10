using System;
using Microsoft.Extensions.Logging;
using Nano.Controllers.Criterias.Interfaces;
using Nano.Eventing.Interfaces;
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
        protected DefaultControllerSpatial(ILogger logger, IServiceSpatial service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }
    }
}