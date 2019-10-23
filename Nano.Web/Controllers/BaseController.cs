using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;
using Nano.Security.Extensions;
using Nano.Web.Const;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Base controller.
    /// </summary>
    [Route("[controller]")]
    [Authorize(Roles = BuiltInUserRoles.Administrator + "," + BuiltInUserRoles.Service)]
    public abstract class BaseController : Controller
    {
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
        /// Options. Any route can be called with http options, to return options header information.
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
            return this.Ok("OK");
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
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Eventing.
        /// </summary>
        protected virtual IEventing Eventing { get; }

        /// <summary>
        /// Repository.
        /// </summary>
        protected virtual TRepository Repository { get; }

        /// <summary>
        /// Constructor accepting an instance of <typeparamref name="TRepository"/> and initializing <see cref="Repository"/>
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="repository">The <see cref="IRepository"/>.</param>
        /// <param name="eventing">The <see cref="IEventingProvider"/>.</param>
        protected BaseController(ILogger logger, TRepository repository, IEventing eventing)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Repository = repository;
            this.Eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));
        }
    }
}