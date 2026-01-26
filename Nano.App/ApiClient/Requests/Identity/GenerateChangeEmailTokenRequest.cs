using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateChangeEmailTokenRequest : GenerateChangeEmailTokenRequest<Guid>;

/// <summary>
/// Request to generate a change email token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GenerateChangeEmailTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change email token information.
    /// </summary>
    public virtual GenerateChangeEmailToken<TIdentity> ChangeEmailToken { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateChangeEmailTokenRequest{TIdentity}"/>.
    /// Sets the action to "email/change/token".
    /// </summary>
    public GenerateChangeEmailTokenRequest()
    {
        this.Action = "email/change/token";
    }

    /// <summary>
    /// Gets the request body containing the change email token.
    /// </summary>
    public override object GetBody()
    {
        return this.ChangeEmailToken;
    }
}