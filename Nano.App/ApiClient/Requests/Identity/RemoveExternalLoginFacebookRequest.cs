using System;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for remove external login Facebook requests.
/// </summary>
public class RemoveExternalLoginFacebookRequest<TIdentity>() : BaseAddExternalLoginRequest<AuthCodeFlow, TIdentity>(BuiltInExternalLogInProviderNames.FACEBOOK)
    where TIdentity : IEquatable<TIdentity>;