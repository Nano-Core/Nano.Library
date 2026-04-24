using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for implicit external sign-up requests.
/// </summary>
public class SignUpExternalImplicitRequest<TUser, TIdentity>(string providerName) : BaseSignUpExternalRequest<ImplicitFlow, TUser, TIdentity>(providerName)
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>;