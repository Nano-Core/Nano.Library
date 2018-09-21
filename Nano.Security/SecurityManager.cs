using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nano.Models;
using Nano.Security.Extensions;

namespace Nano.Security
{
    /// <summary>
    /// Security Manager.
    /// </summary>
    public class SecurityManager
    {
        /// <summary>
        /// User Manager.
        /// </summary>
        protected virtual UserManager<IdentityUser> UserManager { get; }

        /// <summary>
        /// Sign In Manager.
        /// </summary>
        protected virtual SignInManager<IdentityUser> SignInManager { get; }

        /// <summary>
        /// Options.
        /// </summary>
        protected virtual SecurityOptions Options { get; }

        /// <summary>
        /// The user authenticates and on success recieves a jwt token for use with auhtorization.
        /// </summary>
        /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
        /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
        /// <param name="options">The <see cref="SecurityOptions"/>.</param>
        public SecurityManager(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, SecurityOptions options)
        {
            if (signInManager == null)
                throw new ArgumentNullException(nameof(signInManager));

            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.Options = options;
        }

        /// <summary>
        /// The user authenticates and on success recieves a jwt token for use with auhtorization.
        /// </summary>
        /// <param name="login"></param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignInAsync(Login login)
        {
            if (!this.Options.IsEnabled)
                return new AccessToken();

            var result = await this.SignInManager
                .PasswordSignInAsync(login.Username, login.Password, false, false);

            if (!result.Succeeded)
                throw new UnauthorizedAccessException();

            var user = this.UserManager.Users
                .SingleOrDefault(x => x.UserName == login.Username);

            return await this.UserManager
                .GenerateJwtToken(user, this.Options);
        }

        /// <summary>
        /// The user is logged out, and the token is invalidated.
        /// </summary>
        /// <returns></returns>
        public virtual async Task SignOutAsync()
        {
            if (!this.Options.IsEnabled)
                return;

            await this.SignInManager.SignOutAsync();
        }
    }
}