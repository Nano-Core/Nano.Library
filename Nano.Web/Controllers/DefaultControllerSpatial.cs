using System;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models.Criterias.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerSpatial<TEntity, TCriteria> : DefaultControllerSpatial<TEntity, Guid, TCriteria>
        where TEntity : class, IEntityIdentity<Guid>, IEntitySpatial, IEntityWritable
        where TCriteria : class, IQueryCriteriaSpatial, new()
    {
        /// <inheritdoc />
        protected DefaultControllerSpatial(ILogger logger, IRepositorySpatial repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }

    /// <inheritdoc />
    public class DefaultControllerSpatial<TEntity, TIdentity, TCriteria> : BaseControllerSpatialWritable<IRepositorySpatial, TEntity, TIdentity, TCriteria>
        where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial, IEntityWritable
        where TCriteria : class, IQueryCriteriaSpatial, new()
    {
        /// <inheritdoc />
        protected DefaultControllerSpatial(ILogger logger, IRepositorySpatial repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }
}