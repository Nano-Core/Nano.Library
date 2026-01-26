using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to set a user's password.
/// </summary>
public class SetPasswordRequest : BaseRequestPost
{
    /// <summary>
    /// Contains the password information to set.
    /// </summary>
    public virtual SetPassword<Guid> SetPassword { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="SetPasswordRequest"/> with action set.
    /// </summary>
    public SetPasswordRequest()
    {
        this.Action = "password/set";
    }

    /// <summary>
    /// Gets the body of the request containing the password data.
    /// </summary>
    public override object GetBody()
    {
        return this.SetPassword;
    }
}