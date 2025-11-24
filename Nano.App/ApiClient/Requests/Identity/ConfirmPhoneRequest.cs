using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ConfirmPhoneRequest : ConfirmPhoneRequest<Guid>;

/// <inheritdoc />
public class ConfirmPhoneRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Confirm Phone.
    /// </summary>
    public virtual ConfirmPhoneNumber<TIdentity> ConfirmPhone { get; set; } = new();

    /// <inheritdoc />
    public ConfirmPhoneRequest()
    {
        this.Action = "phone/confirm";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ConfirmPhone;
    }
}