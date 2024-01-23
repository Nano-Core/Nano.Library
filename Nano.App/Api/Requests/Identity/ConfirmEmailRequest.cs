using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class ConfirmEmailRequest : ConfirmEmailRequest<Guid>;

/// <inheritdoc />
public class ConfirmEmailRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Confirm Email.
    /// </summary>
    public virtual ConfirmEmail<TIdentity> ConfirmEmail { get; set; } = new();

    /// <inheritdoc />
    public ConfirmEmailRequest()
    {
        this.Action = "email/confirm";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ConfirmEmail;
    }
}