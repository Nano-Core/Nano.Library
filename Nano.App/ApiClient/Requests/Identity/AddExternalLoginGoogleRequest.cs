using Nano.Data.Abstractions.Identity.Authentication.Consts;
using System;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for add external login Google request.
/// </summary>
public class AddExternalLoginGoogleRequest<TIdentity>() : AddExternalLoginImplicitRequest<TIdentity>(BuiltInExternalLogInProviderNames.GOOGLE)
    where TIdentity : IEquatable<TIdentity>;