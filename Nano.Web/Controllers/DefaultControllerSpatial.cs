using System;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Criterias.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerSpatial<TEntity, TCriteria> : BaseControllerSpatial<IRepositorySpatial, TEntity, Guid, TCriteria>
        where TEntity : DefaultEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial, new()
    {
        /// <inheritdoc />
        protected DefaultControllerSpatial(ILogger logger, IRepositorySpatial repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }
}