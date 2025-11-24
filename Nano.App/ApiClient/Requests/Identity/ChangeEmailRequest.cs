using System;
using Nano.App.ApiClient.Requests.Attributes;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

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