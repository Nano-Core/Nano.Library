using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Controllers
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