using System;
using Microsoft.Extensions.Logging;
using Nano.Controllers.Criterias.Interfaces;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Services.Interfaces;

namespace Nano.Controllers
{
    /// <inheritdoc />
    public class DefaultController<TEntity, TCriteria> : BaseController<IService, TEntity, Guid, TCriteria>
        where TEntity : DefaultEntity
        where TCriteria : class, ICriteria
    {
        /// <inheritdoc />
        protected DefaultController(ILogger logger, IService service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }
    }
}