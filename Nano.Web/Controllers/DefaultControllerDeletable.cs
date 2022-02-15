using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerDeletable<TEntity, TCriteria> : DefaultControllerDeletable<TEntity, Guid, TCriteria>
        where TEntity : class, IEntityIdentity<Guid>, IEntityDeletable, new()
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultControllerDeletable(ILogger logger, IRepository repository)
            : this(logger, repository, new NullEventing())
        {

        }

        /// <inheritdoc />
        protected DefaultControllerDeletable(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }

    /// <inheritdoc />
    public class DefaultControllerDeletable<TEntity, TIdentity, TCriteria> : BaseControllerDeletable<IRepository, TEntity, TIdentity, TCriteria>
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable, new()
        where TCriteria : class, IQueryCriteria, new()
        where TIdentity : IEquatable<TIdentity>
    {
        /// <inheritdoc />
        protected DefaultControllerDeletable(ILogger logger, IRepository repository)
            : this(logger, repository, new NullEventing())
        {

        }

        /// <inheritdoc />
        protected DefaultControllerDeletable(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }
}