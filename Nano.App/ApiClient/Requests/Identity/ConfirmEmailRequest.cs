using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ConfirmEmailRequest : ConfirmEmailRequest<Guid>;

/// <summary>
/// Request to confirm a user's email address.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class ConfirmEmailRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The confirm email token information.
    /// </summary>
    public virtual ConfirmEmail<TIdentity> ConfirmEmail { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ConfirmEmailRequest{TIdentity}"/>.
    /// Sets the action to "email/confirm".
    /// </summary>
    public ConfirmEmailRequest()
    {
        this.Action = "email/confirm";
    }

    /// <summary>
    /// Gets the request body containing the confirm email token.
    /// </summary>
    public override object GetBody()
    {
        return this.ConfirmEmail;
    }
}