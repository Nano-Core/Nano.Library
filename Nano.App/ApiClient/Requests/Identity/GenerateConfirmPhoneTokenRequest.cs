using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateConfirmPhoneTokenRequest : GenerateConfirmPhoneTokenRequest<Guid>;

/// <summary>
/// Request to generate a confirm phone token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GenerateConfirmPhoneTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The confirm phone token information.
    /// </summary>
    public virtual GenerateConfirmPhoneToken<TIdentity> ConfirmPhoneToken { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateConfirmPhoneTokenRequest{TIdentity}"/>.
    /// Sets the action to "phone/confirm/token".
    /// </summary>
    public GenerateConfirmPhoneTokenRequest()
    {
        this.Action = "phone/confirm/token";
    }

    /// <summary>
    /// Gets the request body containing the confirm phone token.
    /// </summary>
    public override object GetBody()
    {
        return this.ConfirmPhoneToken;
    }
}