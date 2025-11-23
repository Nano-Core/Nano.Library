using System;
using Nano.App.Web.Identity.Abstractions;
using Nano.Web;

namespace Nano.App.Web.Identity;

/// <inheritdoc />
public class DefaultIdentityAuthTransientRepository<TIdentity> : BaseIdentityAuthTransientRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public DefaultIdentityAuthTransientRepository(IdentityOptions options, IIdentityJwtRepository<TIdentity> identityJwtRepository)
        : base(options, identityJwtRepository)
    {
    }
}