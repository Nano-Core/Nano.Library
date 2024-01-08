using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class ChangePhoneRequest : ChangePhoneRequest<Guid>;

/// <inheritdoc />
public class ChangePhoneRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Change Phone.
    /// </summary>
    public virtual ChangePhoneNumber<TIdentity> ChangePhone { get; set; } = new();

    /// <inheritdoc />
    public ChangePhoneRequest()
    {
        this.Action = "phone/change";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ChangePhone;
    }
}