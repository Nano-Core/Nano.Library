using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nano.Models;
using Nano.Services.Data;
using Nano.Web.Controllers.Criterias;
using Nano.Web.Controllers.Extensions;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    [Authorize]
    [Route("[controller]")]
    public class AuditController : Controller
    {
        /// <summary>
        /// Context.
        /// </summary>
        public virtual DefaultDbContext Context { get; set; }

        /// <inheritdoc />
        public AuditController(DefaultDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.Context = context;
        }

        /// <summary>
        /// Gets all models. 
        /// Filtered by query model parameters (pagination and ordering).
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>A collection of models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpGet]
        [HttpPost]
        [Route("index")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Index([FromQuery][FromBody]Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Context.__EFAudit
                .Order(query.Order)
                .Limit(query.Paging)
                .Include(x => x.Properties)
                .ToArrayAsync(cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the model, uniquely identified by the supplied identifier.
        /// </summary>
        /// <param name="id">The identifier, that uniquely identifies the model.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Details about the model.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="404">No results found.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpGet]
        [Route("details/{id}")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Details([FromRoute][Required]int id, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Context.__EFAudit
                .FindAsync(new[] { (object)id }, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, uniquely identified by the supplied array of identifiers.
        /// </summary>
        /// <param name="ids">The identifier, that uniquely identifies the model.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpPost]
        [Route("details")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Details([FromForm][FromBody][Required]int[] ids, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Context.__EFAudit
                .Where(x => ids.Any(y => y == x.AuditEntryID))
                .ToArrayAsync(cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, matching the query criteria, pagination and ordering.
        /// </summary>
        /// <param name="query">The query criteria model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpPost]
        [Route("query")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Query([FromForm][FromBody][Required]Query<AuditEntryQueryCriteria> query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Context
                .__EFAudit
                .Where(query.Criteria)
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }
    }
}