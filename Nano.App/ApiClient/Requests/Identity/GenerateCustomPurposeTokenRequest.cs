using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateCustomPurposeTokenRequest : GenerateCustomPurposeTokenRequest<Guid>;

/// <summary>
/// Request to generate a custom purpose token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GenerateCustomPurposeTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The custom purpose token information.
    /// </summary>
    public virtual GenerateCustomPurposeToken<TIdentity> CustomPurposeToken { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="GenerateCustomPurposeTokenRequest{TIdentity}"/>.
    /// Sets the action to "token/custom-purpose".
    /// </summary>
    public GenerateCustomPurposeTokenRequest()
    {
        this.Action = "token/custom-purpose";
    }

    /// <summary>
    /// Gets the request body containing the custom purpose token.
    /// </summary>
    public override object GetBody()
    {
        return this.CustomPurposeToken;
    }
}