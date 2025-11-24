using System;
using Nano.App.ApiClient.Models.Identity.External.Providers;

namespace Nano.App.ApiClient.Models.Identity;

/// <summary>
/// Add External Login Facebook.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginFacebook<TIdentity> : AddExternalLoginImplicit<ExternalLoginProviderFacebook, TIdentity>
    where TIdentity : IEquatable<TIdentity>;