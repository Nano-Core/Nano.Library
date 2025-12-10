using System;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Abstractions;

namespace Nano.App.Web.Identity;

/// <inheritdoc cref="IAuthTransientRepository" />
public class DefaultAuthTransientRepository : BaseAuthTransientRepository<Guid>, IAuthTransientRepository
{
    /// <inheritdoc />
    public DefaultAuthTransientRepository(IdentityOptions options, IIdentityJwtRepository identityJwtRepository)
        : base(options, identityJwtRepository)
    {
    }
}

/// <inheritdoc />
public class DefaultAuthTransientRepository<TIdentity> : BaseAuthTransientRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public DefaultAuthTransientRepository(IdentityOptions options, IIdentityJwtRepository<TIdentity> identityJwtRepository)
        : base(options, identityJwtRepository)
    {
    }
}