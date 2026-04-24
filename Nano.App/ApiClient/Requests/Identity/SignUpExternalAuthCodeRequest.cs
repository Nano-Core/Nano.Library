using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for auth-code external sign-up requests.
/// </summary>
public class SignUpExternalAuthCodeRequest<TUser, TIdentity>(string providerName) : BaseSignUpExternalRequest<AuthCodeFlow, TUser, TIdentity>(providerName)
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>;