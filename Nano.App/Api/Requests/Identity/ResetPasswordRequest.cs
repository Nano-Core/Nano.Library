using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class ResetPasswordRequest : ResetPasswordRequest<Guid>;

/// <inheritdoc />
public class ResetPasswordRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Reset Password.
    /// </summary>
    public virtual ResetPassword<TIdentity> ResetPassword { get; set; } = new();

    /// <inheritdoc />
    public ResetPasswordRequest()
    {
        this.Action = "password/reset";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ResetPassword;
    }
}