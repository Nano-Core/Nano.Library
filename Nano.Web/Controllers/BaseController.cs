using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Models.Eventing.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;
using Nano.Security.Extensions;

namespace Nano.Web.Controllers;

/// <summary>
/// Base controller.
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("v{v:apiVersion}/[controller]")]
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