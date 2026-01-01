using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AddExternalLoginAuthCode<TProvider, TIdentity> : BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : ExternalLoginProviderAuthCode, new()
    where TIdentity : IEquatable<TIdentity>;