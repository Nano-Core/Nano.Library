using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerUpdatable<TEntity, TCriteria> : DefaultControllerUpdatable<TEntity, Guid, TCriteria>
        where TEntity : class, IEntityIdentity<Guid>, IEntityUpdatable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultControllerUpdatable(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }

    /// <inheritdoc />
    public class DefaultControllerUpdatable<TEntity, TIdentity, TCriteria> : BaseControllerUpdatable<IRepository, TEntity, TIdentity, TCriteria>
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityUpdatable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultControllerUpdatable(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }
}