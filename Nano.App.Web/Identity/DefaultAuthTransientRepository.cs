using System;
using Nano.App.Web.Identity.Abstractions;
using Nano.Web;

namespace Nano.App.Web.Identity;

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