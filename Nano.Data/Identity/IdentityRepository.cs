using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Models.Identity;

namespace Nano.Data.Identity;

/// <inheritdoc />
public class IdentityRepository<TIdentity>(IOptionsMonitor<DataOptions> options, BaseDbContext<TIdentity> dbContext, SignInManager<IdentityUserEx<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUserEx<TIdentity>> userManager)
    : BaseIdentityRepository<TIdentity>(options, dbContext, signInManager, userManager, roleManager)
    where TIdentity : IEquatable<TIdentity>;

/// <inheritdoc cref="IIdentityRepository"/>
public class IdentityRepository(IOptionsMonitor<DataOptions> options, BaseDbContext<Guid> dbContext, SignInManager<IdentityUserEx<Guid>> signInManager, UserManager<IdentityUserEx<Guid>> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    : BaseIdentityRepository<Guid>(options, dbContext, signInManager, userManager, roleManager), IIdentityRepository;