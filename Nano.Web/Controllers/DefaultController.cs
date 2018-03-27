using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Services.Interfaces;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultController<TEntity, TCriteria> : BaseControllerWritable<IService, TEntity, Guid, TCriteria>
        where TEntity : DefaultEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected DefaultController(ILogger logger, IService service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }
    }
}