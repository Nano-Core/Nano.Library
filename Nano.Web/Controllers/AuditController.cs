using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nano.Data;
using Nano.Models;
using Nano.Models.Criterias;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Extensions;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class AuditController : BaseController
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
        [Route("index")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Index([FromQuery][Required]Query query, CancellationToken cancellationToken = default)
        {
            query = query ?? new Query();

            var result = await this.Context.__EFAudit
                .Order(query.Order)
                .Limit(query.Paging)
                .Include(x => x.Properties)
                .ToArrayAsync(cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
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
        [HttpPost]
        [Route("index")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> IndexPost([FromBody][Required]Query query, CancellationToken cancellationToken = default)
        {
            query = query ?? new Query();

            var result = await this.Context.__EFAudit
                .Order(query.Order)
                .Limit(query.Paging)
                .Include(x => x.Properties)
                .ToArrayAsync(cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

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
        public virtual async Task<IActionResult> Details([FromRoute][Required]int id, CancellationToken cancellationToken = default)
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
        public virtual async Task<IActionResult> DetailsPost([FromBody][Required]int[] ids, CancellationToken cancellationToken = default)
        {
            var result = await this.Context.__EFAudit
                .Where(x => ids.Any(y => y == x.AuditEntryID))
                .ToArrayAsync(cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Details", result);

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
        [HttpGet]
        [Route("query")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Query([FromQuery][Required]Query<AuditEntryQueryCriteria> query, CancellationToken cancellationToken = default)
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
        public virtual async Task<IActionResult> QueryPost([FromBody][Required]Query<AuditEntryQueryCriteria> query, CancellationToken cancellationToken = default)
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