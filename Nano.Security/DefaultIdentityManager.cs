using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Nano.Security;

/// <inheritdoc />
public class DefaultIdentityManager : BaseIdentityManager<Guid>
{
    /// <inheritdoc />
    public DefaultIdentityManager(ILogger logger, DbContext dbContext, SignInManager<IdentityUser<Guid>> signInManager, RoleManager<IdentityRole<Guid>> roleManager, UserManager<IdentityUser<Guid>> userManager, SecurityOptions options)
        : base(logger, dbContext, signInManager, roleManager, userManager, options)
    {
    }
}