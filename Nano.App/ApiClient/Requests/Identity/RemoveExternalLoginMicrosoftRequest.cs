using System;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for remove external login Microsoft requests.
/// </summary>
public class RemoveExternalLoginMicrosoftRequest<TIdentity>() : BaseAddExternalLoginRequest<AuthCodeFlow, TIdentity>(BuiltInExternalLogInProviderNames.MICROSOFT)
    where TIdentity : IEquatable<TIdentity>;