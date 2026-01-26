using System;
using Nano.App.ApiClient.Requests.Annotations;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ChangeEmailRequest : ChangeEmailRequest<Guid>;

/// <summary>
/// Request to change a user's email address.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class ChangeEmailRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change email token information.
    /// </summary>
    public virtual ChangeEmail<TIdentity> ChangeEmail { get; set; } = new();

    /// <summary>
    /// Indicates whether to also set the username when changing email.
    /// </summary>
    [Query]
    public virtual bool SetUsername { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of <see cref="ChangeEmailRequest{TIdentity}"/>.
    /// Sets the action to "email/change".
    /// </summary>
    public ChangeEmailRequest()
    {
        this.Action = "email/change";
    }

    /// <summary>
    /// Gets the request body containing the change email token.
    /// </summary>
    public override object GetBody()
    {
        return this.ChangeEmail;
    }
}