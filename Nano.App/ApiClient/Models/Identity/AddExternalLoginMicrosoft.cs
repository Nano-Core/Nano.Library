using System;
using Nano.App.ApiClient.Models.Identity.External.Providers;

namespace Nano.App.ApiClient.Models.Identity;

/// <summary>
/// Add External Login Microsoft.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginMicrosoft<TIdentity> : AddExternalLoginAuthCode<ExternalLoginProviderMicrosoft, TIdentity>
    where TIdentity : IEquatable<TIdentity>;