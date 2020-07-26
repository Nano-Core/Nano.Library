using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerDeletable<TEntity, TCriteria> : DefaultControllerDeletable<TEntity, Guid, TCriteria>
        where TEntity : class, IEntityIdentity<Guid>, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultControllerDeletable(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }

    /// <inheritdoc />
    public class DefaultControllerDeletable<TEntity, TIdentity, TCriteria> : BaseControllerDeletable<IRepository, TEntity, TIdentity, TCriteria>
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultControllerDeletable(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }
}