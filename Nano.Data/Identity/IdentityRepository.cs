using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Models.Identity;
using System;

namespace Nano.Data.Identity;

/// <inheritdoc />
public class IdentityRepository<TIdentity>(IOptionsMonitor<DataOptions> options, IAuthenticationSchemeProvider schemeProvider, BaseDbContext<TIdentity> dbContext, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUserEx<TIdentity>> userManager)
    : BaseIdentityRepository<TIdentity>(options, schemeProvider, dbContext, userManager, roleManager)
    where TIdentity : IEquatable<TIdentity>;

/// <inheritdoc cref="IIdentityRepository"/>
public class IdentityRepository(IOptionsMonitor<DataOptions> options, IAuthenticationSchemeProvider schemeProvider, BaseDbContext<Guid> dbContext, UserManager<IdentityUserEx<Guid>> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    : BaseIdentityRepository<Guid>(options, schemeProvider, dbContext, userManager, roleManager), IIdentityRepository;