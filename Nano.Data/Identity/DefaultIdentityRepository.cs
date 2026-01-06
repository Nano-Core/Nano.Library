using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Models.Identity;

namespace Nano.Data.Identity;

/// <inheritdoc />
public class DefaultIdentityRepository<TIdentity> : BaseIdentityRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public DefaultIdentityRepository(IOptionsMonitor<DataOptions> options, BaseDbContext<TIdentity> dbContext, SignInManager<IdentityUserEx<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUserEx<TIdentity>> userManager)
        : base(options, dbContext, signInManager, userManager, roleManager)
    {
    }
}

/// <inheritdoc cref="IIdentityRepository"/>
public class DefaultIdentityRepository : BaseIdentityRepository<Guid>, IIdentityRepository
{
    /// <inheritdoc />
    public DefaultIdentityRepository(IOptionsMonitor<DataOptions> options, BaseDbContext<Guid> dbContext, SignInManager<IdentityUserEx<Guid>> signInManager, UserManager<IdentityUserEx<Guid>> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        : base(options, dbContext, signInManager, userManager, roleManager)
    {
    }
}