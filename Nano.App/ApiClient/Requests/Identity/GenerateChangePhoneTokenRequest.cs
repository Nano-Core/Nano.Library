using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateChangePhoneTokenRequest : GenerateChangePhoneTokenRequest<Guid>;

/// <summary>
/// Request to generate a change phone token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GenerateChangePhoneTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change phone token information.
    /// </summary>
    public virtual GenerateChangePhoneToken<TIdentity> ChangePhoneToken { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateChangePhoneTokenRequest{TIdentity}"/>.
    /// Sets the action to "phone/change/token".
    /// </summary>
    public GenerateChangePhoneTokenRequest()
    {
        this.Action = "phone/change/token";
    }

    /// <summary>
    /// Gets the request body containing the change phone token.
    /// </summary>
    public override object GetBody()
    {
        return this.ChangePhoneToken;
    }
}