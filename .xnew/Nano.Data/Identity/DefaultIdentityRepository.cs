using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IdentityOptions = Nano.Data.IdentityOptions;

namespace Nano.Security;

/// <inheritdoc />
public class DefaultIdentityRepository<TIdentity> : BaseIdentityRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public DefaultIdentityRepository(ILogger logger, DbContext dbContext, SignInManager<IdentityUser<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUser<TIdentity>> userManager, IdentityOptions options)
        : base(logger, dbContext, signInManager, roleManager, userManager, options)
    {
    }
}

/// <inheritdoc />
public class DefaultIdentityRepository : BaseIdentityRepository<Guid>
{
    /// <inheritdoc />
    public DefaultIdentityRepository(ILogger logger, DbContext dbContext, SignInManager<IdentityUser<Guid>> signInManager, RoleManager<IdentityRole<Guid>> roleManager, UserManager<IdentityUser<Guid>> userManager, IdentityOptions options)
        : base(logger, dbContext, signInManager, roleManager, userManager, options)
    {
    }
}