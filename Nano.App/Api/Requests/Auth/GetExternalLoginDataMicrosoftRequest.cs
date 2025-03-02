﻿using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataMicrosoftRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderMicrosoft>
{
    /// <inheritdoc />
    public GetExternalLoginDataMicrosoftRequest()
    {
        this.Action = "external/microsoft/data";
    }
}