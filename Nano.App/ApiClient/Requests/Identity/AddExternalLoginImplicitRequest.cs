using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Class for add external login implicit requests.
/// </summary>
public class AddExternalLoginImplicitRequest<TIdentity>(string providerName) : BaseAddExternalLoginRequest<ImplicitFlow, TIdentity>(providerName)
    where TIdentity : IEquatable<TIdentity>;