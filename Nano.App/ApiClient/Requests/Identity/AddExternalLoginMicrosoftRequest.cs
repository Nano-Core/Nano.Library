using Nano.Data.Abstractions.Identity.Authentication.Consts;
using System;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for add external login Microsoft request.
/// </summary>
public class AddExternalLoginMicrosoftRequest<TIdentity>() : AddExternalLoginAuthCodeRequest<TIdentity>(BuiltInExternalLogInProviderNames.MICROSOFT)
    where TIdentity : IEquatable<TIdentity>;