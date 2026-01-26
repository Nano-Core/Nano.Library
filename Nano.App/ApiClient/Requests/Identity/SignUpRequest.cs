using System;
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
public class SignUpRequest<TUser, TIdentity> : BaseRequestPost
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the sign-up information for the user.
    /// </summary>
    public virtual SignUp<TUser, TIdentity> SignUp { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="SignUpRequest{TUser, TIdentity}"/> with action set.
    /// </summary>
    public SignUpRequest()
    {
        this.Action = "signup";
    }

    /// <summary>
    /// Gets the body of the request containing the sign-up information.
    /// </summary>
    public override object GetBody()
    {
        return this.SignUp;
    }
}