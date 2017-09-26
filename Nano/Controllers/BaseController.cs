using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Controllers.Contracts;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Controllers
{
    /// <summary>
    /// Base abstract <see cref="Controller"/>, implementing  methods for instances of <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TService">The <see cref="IService"/> inheriting from <see cref="BaseController{TService,TModel}"/>.</typeparam>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> model the <see cref="IService"/> operates with.</typeparam>
    public abstract class BaseController<TService, TEntity> : Controller
        where TService : IService
        where TEntity : class, IEntityWritable
    {
        /// <summary>
        /// Service.
        /// </summary>
        protected virtual TService Service { get; }

        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Is Xml Content.
        /// </summary>
        protected virtual bool IsHtmlContent => this.Request.ContentType.Contains("text/html");

        /// <summary>
        /// Constructor accepting an instance of <typeparamref name="TService"/> and initializing <see cref="Service"/>
        /// </summary>
        /// <param name="logger">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="service">The <see cref="IService"/>.</param>
        protected BaseController(ILogger<Controller> logger, TService service)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (service == null)
                throw new ArgumentNullException(nameof(service));

            this.Service = service;
            this.Logger = logger;
        }

        /// <summary>
        /// Index.
        /// Gets all instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [FormatFilter]
        public virtual async Task<IActionResult> Index([FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.TraceIdentifier))
            {
                try
                {
                    if (!this.ModelState.IsValid)
                        return this.BadRequest(this.ModelState);

                    var result = await this.Service
                        .GetAll<TEntity>(paging, cancellationToken);

                    if (!this.IsHtmlContent)
                        return this.Ok(result);

                    return this.View(result);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);

                    return this.BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Details.
        /// Gets details about an instance of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> identifier of the <typeparamref name="TEntity"/> instance to det details about.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Details' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [FormatFilter]
        public virtual async Task<IActionResult> Details([FromRoute][FromQuery]Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.TraceIdentifier))
            {
                try
                {
                    if (!this.ModelState.IsValid)
                        return BadRequest(this.ModelState);

                    var result = await this.Service
                        .Get<TEntity>(id, cancellationToken);

                    if (!this.IsHtmlContent)
                        return this.Ok(result);

                    return this.View(result);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);

                    return this.BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Query.
        /// Queries instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Criteria"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [FormatFilter]
        public virtual async Task<IActionResult> Query([FromBody][FromForm]Criteria criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.TraceIdentifier))
            {
                try
                {
                    if (!this.ModelState.IsValid)
                        return this.BadRequest(this.ModelState);

                    var result = await this.Service
                        .GetMany<TEntity>(criteria, paging, cancellationToken);

                    if (!this.IsHtmlContent)
                        return this.Ok(result);

                    return this.View("Index", result);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);

                    return this.BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Create.
        /// Returns 'Create' view for the model of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <returns>The 'Create' <see cref="IActionResult"/></returns>
        [HttpGet]
        [FormatFilter]
        public virtual async Task<IActionResult> Create(CancellationToken cancellationToken = new CancellationToken())
        {
            return await Task.Factory.StartNew<IActionResult>(x =>
            {
                using (this.Logger.BeginScope(this.HttpContext.Session.Id))
                {
                    try
                    {
                        if (!this.ModelState.IsValid)
                            return this.BadRequest(this.ModelState);

                        return this.View();
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex.Message, ex);

                        return this.BadRequest(ex.Message);
                    }
                }
            }, null, cancellationToken);
        }

        /// <summary>
        /// Create.
        /// Creates an instance of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [FormatFilter]
        public virtual async Task<IActionResult> Create([FromBody][FromForm]TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    if (!this.ModelState.IsValid)
                        return this.BadRequest(this.ModelState);

                    await this.Service
                        .Add(entity, cancellationToken);

                    if (!this.IsHtmlContent)
                        return this.Ok(entity);

                    return this.RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);

                    return this.BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Edit.
        /// Returns 'Edit' view for the model of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <returns>The 'Edit' <see cref="IActionResult"/></returns>
        [HttpGet]
        [FormatFilter]
        public virtual IActionResult Edit(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    if (!this.ModelState.IsValid)
                        return this.BadRequest(this.ModelState);

                    var entity = this.Service
                        .Get<TEntity>(id, cancellationToken);

                    return this.View(entity);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);

                    return this.BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Edit.
        /// Creates the instance of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="entity">The <see cref="IEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpPut]
        [FormatFilter]
        public virtual async Task<IActionResult> Edit([FromRoute][FromQuery]Guid id, [FromBody][FromForm]TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    if (!this.ModelState.IsValid)
                        return this.BadRequest(this.ModelState);

                    await this.Service
                        .Update(entity, cancellationToken);

                    if (!this.IsHtmlContent)
                        return this.Ok(entity);

                    return this.RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);

                    return this.BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Delete.
        /// Returns 'Delete' view for the model of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Delete' <see cref="IActionResult"/>.</returns>
        [HttpDelete]
        [FormatFilter]
        public virtual async Task<IActionResult> Delete([FromRoute][FromQuery]Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    if (!this.ModelState.IsValid)
                        return this.BadRequest(this.ModelState);

                    var entity = await this.Service
                        .Get<TEntity>(id, cancellationToken);

                    if (entity == null)
                        return this.NotFound(id);

                    return this.View(entity);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);

                    return this.BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Delete.
        /// Deletes the instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Delete' <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ActionName("Delete")]
        [FormatFilter]
        public virtual async Task<IActionResult> DeleteConfirmed([FromRoute][FromQuery]Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    if (!this.ModelState.IsValid)
                        return this.BadRequest(this.ModelState);

                    await this.Service
                        .Get<TEntity>(id, cancellationToken)
                        .ContinueWith(async x => await this.Service.Delete(x.Result, cancellationToken), cancellationToken);

                    if (!this.IsHtmlContent)
                        return this.Ok(id);

                    this.TempData["SuccessMessage"] = "Deleted Sucessfully";

                    return this.RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);

                    return this.BadRequest(ex.Message);
                }
            }
        }
    }
}