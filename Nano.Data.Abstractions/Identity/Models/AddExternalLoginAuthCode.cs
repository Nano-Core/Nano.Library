using System;
using Nano.Common.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AddExternalLoginAuthCode<TProvider, TIdentity> : BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : ExternalLoginProviderAuthCode, new()
    where TIdentity : IEquatable<TIdentity>;