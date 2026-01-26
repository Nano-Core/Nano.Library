using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ChangePasswordRequest : ChangePasswordRequest<Guid>;

/// <summary>
/// Request to change a user's password.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class ChangePasswordRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change password token information.
    /// </summary>
    public virtual ChangePassword<TIdentity> ChangePassword { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ChangePasswordRequest{TIdentity}"/>.
    /// Sets the action to "password/change".
    /// </summary>
    public ChangePasswordRequest()
    {
        this.Action = "password/change";
    }

    /// <summary>
    /// Gets the request body containing the change password token.
    /// </summary>
    public override object GetBody()
    {
        return this.ChangePassword;
    }
}