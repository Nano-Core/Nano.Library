using System;
using System.Collections.Generic;
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
        protected virtual ILogger<TService> Logger { get; }

        /// <summary>
        /// Constructor accepting an instance of <typeparamref name="TService"/> and initializing <see cref="Service"/>
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="service">The <see cref="IService"/>.</param>
        protected BaseController(ILoggerFactory loggerFactory, TService service)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            if (service == null)
                throw new ArgumentNullException(nameof(service));

            this.Service = service;
            this.Logger = loggerFactory.CreateLogger<TService>();
        }

        // TODO: Error handling (Home/Error/view(html), Return statuscode error for api and also why doesnt development exception pages not work

        // Index => GEtAll
        // Index/{Id}  => get(id)

        /// <summary>
        /// Index.
        /// </summary>
        /// <returns>The 'Index' <see cref="IActionResult"/></returns>
        public virtual async Task<IActionResult> Index()
        {
            var result = await this.Service.GetAll<TEntity>();

            return View(result);
        }

        /// <summary>
        /// Details.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> identifier of the <typeparamref name="TEntity"/> instance to det details about.</param>
        /// <returns>The 'Details' <see cref="IActionResult"/></returns>
        public virtual async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var timeZone = await this.Service.Get<TEntity>(id);

            if (timeZone == null)
                return NotFound();

            return View(timeZone);
        }

        /// <summary>
        /// Create.
        /// </summary>
        /// <returns>The 'Create' <see cref="IActionResult"/></returns>
        public virtual IActionResult Create()
        {
//            this.Response.ContentType = new ContentType().MediaType
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> Create([FromForm]TEntity entity)
        {
            if (!ModelState.IsValid)
                return View(entity);

            await this.Service.Add(entity);

            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> Get([FromRoute]Guid? id, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .Get<TEntity>(id, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }



        /// <summary>
        /// Edit.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var timeZone = await this.Service.Get<TEntity>(id.Value);

            if (timeZone == null)
                return NotFound();

            return View(timeZone);
        }

        /// <summary>
        /// Edit.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> Edit(Guid id, [FromForm]TEntity entity)
        {
            //if (id != entity.Id)
            //    return NotFound();

            if (!ModelState.IsValid)
                return View(entity);

            await this.Service.Update(entity);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var timeZone = await this.Service.Get<TEntity>(id.Value);

            if (timeZone == null)
                return NotFound();

            return View(timeZone);
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public virtual async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await this.Service.Get<TEntity>(id)
                .ContinueWith(async x => await this.Service.Delete(x.Result));

            return RedirectToAction("Index");
        }




        /// <summary>
        /// Gets all <see cref="IEntity"/> instances.
        /// </summary>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        /// <returns></returns>
        [HttpGet("all")]
        public virtual async Task<IEnumerable<TEntity>> GetAll([FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .GetAll<TEntity>(paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IEntity"/>'s instances, that matches the <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Criteria"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("query")]
        public virtual async Task<IEnumerable<TEntity>> GetMany([FromQuery]Criteria criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .GetMany<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Add the passed <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="IEntity"/> to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        [HttpPost]
        public virtual async Task Add([FromBody]TEntity value, CancellationToken cancellationToken = new CancellationToken())
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    await this.Service
                        .Add(value, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Adds the passed <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The <see cref="IEntity"/>'s to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        [HttpPost]
        public virtual async Task Add([FromBody]IEnumerable<TEntity> values, CancellationToken cancellationToken = new CancellationToken())
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    await this.Service
                        .AddMany(values, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Edits the passed <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="IEntity"/> to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        [HttpPut]
        public virtual async Task Update([FromBody]TEntity value, CancellationToken cancellationToken = new CancellationToken())
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    await this.Service
                        .Update(value, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Edits the passed <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The <see cref="IEntity"/>'s to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        [HttpPut]
        public virtual async Task Update([FromBody]IEnumerable<TEntity> values, CancellationToken cancellationToken = new CancellationToken())
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    await this.Service
                        .UpdateMany(values, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes the passed <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="IEntity"/> to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        [HttpDelete]
        public virtual async Task Delete([FromBody]TEntity value, CancellationToken cancellationToken = new CancellationToken())
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    await this.Service
                        .Delete(value, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes the passed <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The <see cref="IEntity"/>'s to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        [HttpDelete]
        public virtual async Task Delete([FromBody]IEnumerable<TEntity> values, CancellationToken cancellationToken = new CancellationToken())
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    await this.Service
                        .DeleteMany(values, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }
    }
}