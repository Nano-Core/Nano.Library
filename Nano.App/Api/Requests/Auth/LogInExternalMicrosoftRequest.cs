﻿using Nano.App.Consts;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogInExternalMicrosoftRequest : BaseLogInExternalRequest<LogInExternalMicrosoft>
{
    /// <inheritdoc />
    public LogInExternalMicrosoftRequest()
    {
        this.Action = "login/external/microsoft";
        this.Controller = Constants.AUTH_CONTROLLER_ROUTE;
    }
}