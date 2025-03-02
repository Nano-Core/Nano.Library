using System;

namespace Nano.Security.Models;

/// <summary>
/// Add External Login Facebook.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginFacebook<TIdentity> : AddExternalLoginImplicit<ExternalLoginProviderFacebook, TIdentity>
    where TIdentity : IEquatable<TIdentity>;