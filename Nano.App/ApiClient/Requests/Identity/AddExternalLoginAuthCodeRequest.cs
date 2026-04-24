using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for add external login auth-code requests.
/// </summary>
public class AddExternalLoginAuthCodeRequest<TIdentity>(string providerName) : BaseAddExternalLoginRequest<AuthCodeFlow, TIdentity>(providerName)
    where TIdentity : IEquatable<TIdentity>;