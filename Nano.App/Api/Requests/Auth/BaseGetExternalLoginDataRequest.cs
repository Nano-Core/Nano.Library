﻿using Nano.App.Consts;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public abstract class BaseGetExternalLoginDataRequest<TProvider> : BaseRequestPost
    where TProvider : BaseLogInExternalProvider, new()
{
    /// <summary>
    /// Provider.
    /// </summary>
    public virtual TProvider Provider { get; set; } = new();

    /// <inheritdoc />
    protected BaseGetExternalLoginDataRequest()
    {
        this.Controller = Constants.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Provider;
    }
}