using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateConfirmEmailTokenRequest : GenerateConfirmEmailTokenRequest<Guid>;

/// <summary>
/// Request to generate a confirm email token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GenerateConfirmEmailTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The confirm email token information.
    /// </summary>
    public virtual GenerateConfirmEmailToken<TIdentity> ConfirmEmailToken { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateConfirmEmailTokenRequest{TIdentity}"/>.
    /// Sets the action to "email/confirm/token".
    /// </summary>
    public GenerateConfirmEmailTokenRequest()
    {
        this.Action = "email/confirm/token";
    }

    /// <summary>
    /// Gets the request body containing the confirm email token.
    /// </summary>
    public override object GetBody()
    {
        return this.ConfirmEmailToken;
    }
}