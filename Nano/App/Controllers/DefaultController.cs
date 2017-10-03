using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;

namespace Nano.App.Controllers
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