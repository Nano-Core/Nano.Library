using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Data;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Auth;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Security.Models;
using Nano.Web.Hosting;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultUserController<TEntity, TCriteria> : DefaultController<TEntity, TCriteria>
        where TEntity : DefaultIdentityUser
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <summary>
        /// Security Manager.
        /// </summary>
        protected virtual SecurityManager SecurityManager { get; }

        /// <inheritdoc />
        protected DefaultUserController(ILogger logger, IRepository repository, IEventing eventing, SecurityManager securityManager) 
            : base(logger, repository, eventing)
        {
            this.SecurityManager = securityManager ?? throw new ArgumentNullException(nameof(securityManager));
        }

        /// <summary>
        /// Creates the passed model.
        /// </summary>
        /// <param name="entity">The model to create.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The created model.</returns>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("signup")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUp([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
        {
            return await this.CreateConfirm(entity, cancellationToken);
        }

        /// <summary>
        /// Creates the passed model.
        /// </summary>
        /// <param name="entity">The model to create.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The created model.</returns>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("create")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public override async Task<IActionResult> CreateConfirm([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
        {
            var signUp = new SignUp
            {
                Email = entity.EmailAddress.Email,
                Username = entity.EmailAddress.Email,
                Password = entity.Password,
                ConfirmPassword = entity.Password
            };

            var identityUser = await this.SecurityManager
                .SignUpAsync(signUp, cancellationToken);

            entity.Password = null;
            entity.IdentityUserId = identityUser.Id;

            var result = await this.Repository
                .AddAsync(entity, cancellationToken);

            return this.Created("create", result);
        }

        /// <summary>
        /// Creates the passed models.
        /// </summary>
        /// <param name="entities">The models to create.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("create/Many")]
        [Authorize(Roles = BuiltInUserRoles.Administrator)]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public override async Task<IActionResult> CreateConfirms([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                var signUp = new SignUp
                {
                    Email = entity.EmailAddress.Email,
                    Username = entity.EmailAddress.Email,
                    Password = entity.Password,
                    ConfirmPassword = entity.Password
                };

                var identityUser = await this.SecurityManager
                    .SignUpAsync(signUp, cancellationToken);

                entity.Password = null;
                entity.IdentityUserId = identityUser.Id;

                await this.Repository
                    .AddAsync(entity, cancellationToken);
            }

            return this.Ok();
        }
    }
}