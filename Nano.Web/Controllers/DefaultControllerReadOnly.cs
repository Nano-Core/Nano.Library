using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerReadOnly<TEntity, TCriteria> : BaseControllerReadOnly<IRepository, TEntity, Guid, TCriteria>
        where TEntity : class, IEntityIdentity<Guid>
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultControllerReadOnly(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
    }
}