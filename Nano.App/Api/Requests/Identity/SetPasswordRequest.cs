using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class SetPasswordRequest : BaseRequestPost
{
    /// <summary>
    /// Set Password.
    /// </summary>
    public virtual SetPassword<Guid> SetPassword { get; set; } = new();

    /// <inheritdoc />
    public SetPasswordRequest()
    {
        this.Action = "password/set";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.SetPassword;
    }
}