using Nano.Data.Abstractions.Identity.Authentication.Consts;
using System;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for add external login Facebook request.
/// </summary>
public class AddExternalLoginRequestFacebookRequest<TIdentity>() : AddExternalLoginImplicitRequest<TIdentity>(BuiltInExternalLogInProviderNames.FACEBOOK)
    where TIdentity : IEquatable<TIdentity>;