using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Nano.Security;

/// <inheritdoc />
public class DefaultIdentityAuthRepository<TIdentity> : BaseIdentityAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultIdentityAuthRepository(ILogger logger, DbContext dbContext, SignInManager<IdentityUser<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUser<TIdentity>> userManager, Web.IdentityOptions options)
        : base(logger, dbContext, signInManager, roleManager, userManager, options)
    {
    }
}