using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ConfirmPhoneRequest : ConfirmPhoneRequest<Guid>;

/// <summary>
/// Request to confirm a user's phone number.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class ConfirmPhoneRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The confirm phone token information.
    /// </summary>
    public virtual ConfirmPhoneNumber<TIdentity> ConfirmPhone { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ConfirmPhoneRequest{TIdentity}"/>.
    /// Sets the action to "phone/confirm".
    /// </summary>
    public ConfirmPhoneRequest()
    {
        this.Action = "phone/confirm";
    }

    /// <summary>
    /// Gets the request body containing the confirm phone token.
    /// </summary>
    public override object GetBody()
    {
        return this.ConfirmPhone;
    }
}