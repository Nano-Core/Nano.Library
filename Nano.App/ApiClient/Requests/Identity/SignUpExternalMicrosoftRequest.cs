using System;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for Microsoft external sign-up requests.
/// </summary>
public class SignUpExternalMicrosoftRequest<TUser, TIdentity>() : SignUpExternalAuthCodeRequest<TUser, TIdentity>(BuiltInExternalLogInProviderNames.MICROSOFT)
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>;