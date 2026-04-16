using System;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for Google external sign-up requests.
/// </summary>
public class SignUpExternalGoogleRequest<TUser, TIdentity>() : SignUpExternalImplicitRequest<TUser, TIdentity>(BuiltInExternalLogInProviderNames.GOOGLE)
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>;