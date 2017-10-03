using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;

namespace Nano.App.Controllers
{
    /// <inheritdoc />
    public class DefaultControllerSpatial<TEntity> : BaseControllerSpatial<IServiceSpatial, TEntity, Guid>
        where TEntity : class, IEntityIdentity<Guid>, IEntitySpatial, IEntityWritable
    {
        /// <inheritdoc />
        protected DefaultControllerSpatial(ILogger<Controller> logger, IServiceSpatial service)
            : base(logger, service)
        {

        }
    }
}