using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class VerifyCustomTokenRequest : VerifyCustomTokenRequest<Guid>;

/// <summary>
/// Represents a request to verify a custom-purpose token.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifier.</typeparam>
public class VerifyCustomTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the custom purpose token to verify.
    /// </summary>
    public virtual ConfirmCustomPurpose<TIdentity> ConfirmCustomPurpose { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="VerifyCustomTokenRequest{TIdentity}"/> with action set.
    /// </summary>
    public VerifyCustomTokenRequest()
    {
        this.Action = "token/custom-purpose/confirm";
    }

    /// <summary>
    /// Gets the body of the request containing the token to verify.
    /// </summary>
    public override object GetBody()
    {
        return this.ConfirmCustomPurpose;
    }
}