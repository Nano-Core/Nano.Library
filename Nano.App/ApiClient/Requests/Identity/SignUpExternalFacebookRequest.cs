using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Models.Abstractions;
using System;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for Facebook external sign-up requests.
/// </summary>
public class SignUpExternalFacebookRequest<TUser, TIdentity>() : SignUpExternalImplicitRequest<TUser, TIdentity>(BuiltInExternalLogInProviderNames.FACEBOOK)
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>;