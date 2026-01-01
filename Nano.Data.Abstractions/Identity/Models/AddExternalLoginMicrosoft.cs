using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Add External Login Microsoft.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginMicrosoft<TIdentity> : AddExternalLoginAuthCode<ExternalLoginProviderMicrosoft, TIdentity>
    where TIdentity : IEquatable<TIdentity>;