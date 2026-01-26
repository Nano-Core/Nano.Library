using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ResetPasswordRequest : ResetPasswordRequest<Guid>;

/// <summary>
/// Represents a request to reset a user's password.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
public class ResetPasswordRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the reset password information.
    /// </summary>
    public virtual ResetPassword<TIdentity> ResetPassword { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ResetPasswordRequest{TIdentity}"/> with action set.
    /// </summary>
    public ResetPasswordRequest()
    {
        this.Action = "password/reset";
    }

    /// <summary>
    /// Gets the body of the request containing the reset password data.
    /// </summary>
    public override object GetBody()
    {
        return this.ResetPassword;
    }
}