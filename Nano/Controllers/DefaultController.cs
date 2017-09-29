using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Controllers
{
    /// <inheritdoc />
    public class DefaultController<TEntity> : BaseController<IService, TEntity, Guid>
        where TEntity : class, IEntityIdentity<Guid>, IEntityWritable
    {
        /// <inheritdoc />
        protected DefaultController(ILogger<Controller> logger, IService service)
            : base(logger, service)
        {

        }
    }
}