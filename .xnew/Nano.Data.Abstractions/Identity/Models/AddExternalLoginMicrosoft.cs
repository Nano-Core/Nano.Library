using System;

namespace Nano.Security.Models;

/// <summary>
/// Add External Login Microsoft.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginMicrosoft<TIdentity> : AddExternalLoginAuthCode<ExternalLoginProviderMicrosoft, TIdentity>
    where TIdentity : IEquatable<TIdentity>;