using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nano.Models;

namespace Nano.Security;

/// <inheritdoc />
public class DefaultIdentityManager<TIdentity> : BaseIdentityManager<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public DefaultIdentityManager(ILogger logger, DbContext dbContext, SignInManager<IdentityUserExpanded<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUserExpanded<TIdentity>> userManager, SecurityOptions options)
        : base(logger, dbContext, signInManager, roleManager, userManager, options)
    {
    }
}

/// <inheritdoc />
public class DefaultIdentityManager : BaseIdentityManager<Guid>
{
    /// <inheritdoc />
    public DefaultIdentityManager(ILogger logger, DbContext dbContext, SignInManager<IdentityUserExpanded<Guid>> signInManager, RoleManager<IdentityRole<Guid>> roleManager, UserManager<IdentityUserExpanded<Guid>> userManager, SecurityOptions options)
        : base(logger, dbContext, signInManager, roleManager, userManager, options)
    {
    }
}