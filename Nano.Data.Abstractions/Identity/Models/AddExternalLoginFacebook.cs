using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Add External Login Facebook.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginFacebook<TIdentity> : AddExternalLoginImplicit<ExternalLoginProviderFacebook, TIdentity>
    where TIdentity : IEquatable<TIdentity>;