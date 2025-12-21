using System;
using Nano.App.ApiClient.Models.Identity.External.Providers;

namespace Nano.App.ApiClient.Models.Identity.External;

/// <summary>
/// Add External Login Google.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginGoogle<TIdentity> : AddExternalLoginImplicit<ExternalLoginProviderGoogle, TIdentity>
    where TIdentity : IEquatable<TIdentity>;