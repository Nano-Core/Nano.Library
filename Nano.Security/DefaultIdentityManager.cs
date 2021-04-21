using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Nano.Security
{
    /// <inheritdoc />
    public class DefaultIdentityManager : BaseIdentityManager<Guid>
    {
        /// <inheritdoc />
        public DefaultIdentityManager(DbContext dbContext, SignInManager<IdentityUser<Guid>> signInManager, RoleManager<IdentityRole<Guid>> roleManager, UserManager<IdentityUser<Guid>> userManager, SecurityOptions options) 
            : base(dbContext, signInManager, roleManager, userManager, options)
        {

        }
    }
}