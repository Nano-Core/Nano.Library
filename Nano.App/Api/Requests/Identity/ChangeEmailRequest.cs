using System;
using Nano.App.Api.Requests.Attributes;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class ChangeEmailRequest : ChangeEmailRequest<Guid>;

/// <inheritdoc />
public class ChangeEmailRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Change Email.
    /// </summary>
    public virtual ChangeEmail<TIdentity> ChangeEmail { get; set; } = new();

    /// <summary>
    /// Set Username.
    /// </summary>
    [Query]
    public virtual bool SetUsername { get; set; } = false;

    /// <inheritdoc />
    public ChangeEmailRequest()
    {
        this.Action = "email/change";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ChangeEmail;
    }
}