﻿using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class SignUpExternalGoogleRequest<TUser> : SignUpExternalGoogleRequest<TUser, Guid>
    where TUser : IEntityUser<Guid>, new();

/// <inheritdoc />
public class SignUpExternalGoogleRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalGoogle<TUser, TIdentity>>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public SignUpExternalGoogleRequest()
    {
        this.Action = "signup/external/google";
    }
}