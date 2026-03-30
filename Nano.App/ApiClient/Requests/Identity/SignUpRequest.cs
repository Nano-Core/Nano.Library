using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class SignUpRequest<TUser> : SignUpRequest<TUser, Guid>
    where TUser : IEntityUser<Guid>;

/// <summary>
/// Represents a request to sign up a new user.
/// </summary>
/// <typeparam name="TUser">The type of the user entity.</typeparam>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_SIGNUP)]
public class SignUpRequest<TUser, TIdentity> : BaseRequest
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the sign-up information for the user.
    /// </summary>
    [Required]
    [Body]
    public virtual SignUp<TUser, TIdentity> SignUp { get; set; } = new();
}