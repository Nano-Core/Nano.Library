using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions.Identity.Abstractions;
using IdentityOptions = Nano.Data.Abstractions.Config.IdentityOptions;

namespace Nano.Data.Identity;

/// <inheritdoc />
public class DefaultIdentityRepository<TIdentity> : BaseIdentityRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public DefaultIdentityRepository(IdentityOptions options, BaseDbContext<TIdentity> dbContext, SignInManager<IdentityUser<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUser<TIdentity>> userManager)
        : base(options, dbContext, signInManager, userManager, roleManager)
    {
    }
}

/// <inheritdoc cref="IIdentityRepository"/>
public class DefaultIdentityRepository : BaseIdentityRepository<Guid>, IIdentityRepository
{
    /// <inheritdoc />
    public DefaultIdentityRepository(IdentityOptions options, BaseDbContext<Guid> dbContext, SignInManager<IdentityUser<Guid>> signInManager, UserManager<IdentityUser<Guid>> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        : base(options, dbContext, signInManager, userManager, roleManager)
    {
    }
}