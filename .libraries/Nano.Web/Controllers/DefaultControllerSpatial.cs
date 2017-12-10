using System;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Criterias.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerSpatial<TEntity, TCriteria> : BaseControllerSpatial<IServiceSpatial, TEntity, Guid, TCriteria>
        where TEntity : DefaultEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        /// <inheritdoc />
        protected DefaultControllerSpatial(ILogger logger, IServiceSpatial service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }
    }
}