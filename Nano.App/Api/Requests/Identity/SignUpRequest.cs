﻿using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class SignUpRequest<TUser> : SignUpRequest<TUser, Guid>
    where TUser : IEntityUser<Guid>;

/// <inheritdoc />
public class SignUpRequest<TUser, TIdentity> : BaseRequestPost
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Sign Up.
    /// </summary>
    public virtual SignUp<TUser, TIdentity> SignUp { get; set; } = new();

    /// <inheritdoc />
    public SignUpRequest()
    {
        this.Action = "signup";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.SignUp;
    }
}