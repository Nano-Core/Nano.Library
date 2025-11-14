using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;

namespace Nano.Security;

/// <inheritdoc />
public class DefaultIdentityAuthTransientRepository<TIdentity> : BaseIdentityAuthTransientRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public DefaultIdentityAuthTransientRepository(ILogger logger, Web.IdentityOptions options, SignInManager<IdentityUser<TIdentity>> signInManager)
        : base(logger, options, signInManager)
    {
    }
}