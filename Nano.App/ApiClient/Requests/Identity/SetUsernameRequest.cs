using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class SetUsernameRequest : SetUsernameRequest<Guid>;

/// <summary>
/// Represents a request to set a user's username.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_USERNAME_SET)]
public class SetUsernameRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the username to set.
    /// </summary>
    [Required]
    [Body]
    public virtual SetUsername SetUsername { get; set; } = new();
}