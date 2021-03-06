using System;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Eventing.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;
using Nano.Security.Extensions;
using Nano.Web.Const;

namespace Nano.Web.Controllers
{
    // TODO: Swagger: Triple-slash Xml documentation not working for methods with generic types: https://github.com/domaindrivendev/Swashbuckle.WebApi/issues/749

    /// <summary>
    /// Base controller.
    /// </summary>
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE)]
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// User Id.
        /// </summary>
        public virtual Guid? UserId => this.HttpContext.GetJwtUserId();

        /// <summary>
        /// User Name.
        /// </summary>
        public virtual string UserName => this.HttpContext.GetJwtUserName();

        /// <summary>
        /// User Email.
        /// </summary>
        public virtual string UserEmail => this.HttpContext.GetJwtUserEmail();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        protected BaseController(ILogger logger)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Options.
        /// Any route can be called with http options, to return options header information.
        /// </summary>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpOptions]
        [Route("")]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public virtual IActionResult Options()
        {
            return this.Ok();
        }
    }

    /// <summary>
    /// Base generic controller.
    /// </summary>
    /// <typeparam name="TRepository">The <see cref="IRepository"/>.</typeparam>
    public abstract class BaseController<TRepository> : BaseController
       where TRepository : IRepository
    {
        /// <summary>
        /// Eventing.
        /// </summary>
        protected virtual IEventing Eventing { get; }

        /// <summary>
        /// Repository.
        /// </summary>
        protected virtual TRepository Repository { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="repository">The <see cref="IRepository"/>.</param>
        protected BaseController(ILogger logger, TRepository repository)
            : base(logger)
        {
            this.Repository = repository;
            this.Eventing = new NullEventing();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="repository">The <see cref="IRepository"/>.</param>
        /// <param name="eventing">The <see cref="IEventing"/>.</param>
        protected BaseController(ILogger logger, TRepository repository, IEventing eventing)
            : base(logger)
        {
            this.Repository = repository;
            this.Eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));
        }
    }
}