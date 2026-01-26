using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ChangePhoneRequest : ChangePhoneRequest<Guid>;

/// <summary>
/// Request to change a user's phone number.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class ChangePhoneRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change phone token information.
    /// </summary>
    public virtual ChangePhoneNumber<TIdentity> ChangePhone { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ChangePhoneRequest{TIdentity}"/>.
    /// Sets the action to "phone/change".
    /// </summary>
    public ChangePhoneRequest()
    {
        this.Action = "phone/change";
    }

    /// <summary>
    /// Gets the request body containing the change phone token.
    /// </summary>
    public override object GetBody()
    {
        return this.ChangePhone;
    }
}