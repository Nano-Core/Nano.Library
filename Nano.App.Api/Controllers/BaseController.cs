using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Identity.Authentication.Consts;
using Nano.App.Extensions;
using Nano.Data.Abstractions;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <summary>
/// Base API controller that provides common properties and behaviors for all derived controllers.
/// </summary>
[ApiController]
[Route("[controller]")]
[Route("v{v:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicies.DEFAULT)]
public abstract class BaseController : Controller
{
    /// <summary>
    /// Logger instance for logging messages within the controller.
    /// </summary>
    protected virtual ILogger<BaseController> Logger { get; }

    /// <summary>
    /// Gets the current request identififer from header.
    /// </summary>
    public virtual string? RequestId => this.HttpContext.Request.GetRequestId();

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> used for logging within the controller.</param>
    protected BaseController(ILogger<BaseController> logger)
    {
        this.Logger = logger;
    }
}

/// <summary>
/// Base generic API controller that provides repository and eventing support.
/// </summary>
/// <typeparam name="TRepository">The type of repository implementing <see cref="IRepository"/>.</typeparam>
public abstract class BaseController<TRepository> : BaseController
    where TRepository : class, IRepository
{
    /// <summary>
    /// Repository used for data access operations within the controller.
    /// </summary>
    protected virtual TRepository Repository { get; }

    /// <summary>
    /// Optional eventing interface for publishing domain events.
    /// </summary>
    protected virtual IEventing? Eventing { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController{TRepository}"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> used for logging.</param>
    /// <param name="repository">The repository implementing <see cref="IRepository"/>.</param>
    /// <param name="eventing">Optional <see cref="IEventing"/> for publishing events.</param>
    protected BaseController(ILogger<BaseController<TRepository>> logger, TRepository repository, IEventing? eventing = null)
        : base(logger)
    {
        this.Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.Eventing = eventing;
    }
}