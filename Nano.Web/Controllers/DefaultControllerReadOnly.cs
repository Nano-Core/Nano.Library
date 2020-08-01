using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerReadOnly<TEntity, TCriteria> : DefaultControllerReadOnly<TEntity, Guid, TCriteria>
        where TEntity : class, IEntityIdentity<Guid>
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultControllerReadOnly(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }

    /// <inheritdoc />
    public class DefaultControllerReadOnly<TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<IRepository, TEntity, TIdentity, TCriteria>
        where TEntity : class, IEntityIdentity<TIdentity>
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultControllerReadOnly(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }
}