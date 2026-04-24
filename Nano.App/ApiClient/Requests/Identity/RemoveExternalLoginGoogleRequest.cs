using System;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for remove external login Google requests.
/// </summary>
public class RemoveExternalLoginGoogleRequest<TIdentity>() : BaseAddExternalLoginRequest<AuthCodeFlow, TIdentity>(BuiltInExternalLogInProviderNames.GOOGLE)
    where TIdentity : IEquatable<TIdentity>;