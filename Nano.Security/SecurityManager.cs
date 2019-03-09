using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Auth;
using Nano.Security.Exceptions;
using Nano.Security.Extensions;
using Nano.Security.Models;

namespace Nano.Security
{
    /// <summary>
    /// Security Manager.
    /// </summary>
    public class SecurityManager
    {
        /// <summary>
        /// Options.
        /// </summary>
        protected virtual SecurityOptions Options { get; }

        /// <summary>
        /// User Manager.
        /// </summary>
        protected virtual UserManager<IdentityUser> UserManager { get; }

        /// <summary>
        /// Sign In Manager.
        /// </summary>
        protected virtual SignInManager<IdentityUser> SignInManager { get; }

        /// <summary>
        /// The user authenticates and on success recieves a jwt token for use with auhtorization.
        /// </summary>
        /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
        /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
        /// <param name="options">The <see cref="SecurityOptions"/>.</param>
        public SecurityManager(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, SecurityOptions options)
        {
            this.UserManager = userManager?? throw new ArgumentNullException(nameof(userManager));
            this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// The user authenticates, and on success recieves a jwt token for use with auhtorization.
        /// If two-factor authentication is enabled, a indicating flag is returned, and no access token is generated.
        /// If the user is locked out, or sign-in is unsuccessful, an exception is thrown.
        /// </summary>
        /// <param name="login">The <see cref="Login"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignInAsync(Login login, CancellationToken cancellationToken = default)
        {
            if (login == null) 
                throw new ArgumentNullException(nameof(login));
            
            var result = await this.SignInManager
                .PasswordSignInAsync(login.Username, login.Password, login.IsRememerMe, this.Options.Lockout.AllowedForNewUsers);

            if (result.Succeeded)
            {
                var user = await this.UserManager
                    .FindByNameAsync(login.Username);

                return await this.UserManager
                    .GenerateJwtToken(user, this.Options);
            }

            if (result.IsLockedOut)
                throw new UnauthorizedLockoutException();
            
            throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// The user is logged out, and the token is invalidated.
        /// </summary>
        /// <returns>Void.</returns>
        public virtual async Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            await this.SignInManager
                .SignOutAsync();
        }

        /// <summary>
        /// Registers a new user.
        /// The new user is signed in as well, and an email confirmation is sent, if enabled. 
        /// </summary>
        /// <param name="signUp">The <see cref="SignUp"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityUser"/>.</returns>
        public virtual async Task<IdentityUser> SignUpAsync(SignUp signUp, CancellationToken cancellationToken = default)
        {
            if (signUp == null) 
                throw new ArgumentNullException(nameof(signUp));
            
            var user = new IdentityUser
            {
                Email = signUp.Email,
                UserName = signUp.Username
            };

            var result = await this.UserManager
                .CreateAsync(user, signUp.Password);

            user = await this.UserManager
                .FindByNameAsync(signUp.Username);

            await this.UserManager
                .AddToRolesAsync(user, this.Options.User.DefaultRoles);

            if (!result.Succeeded)
                this.ThrowErrors(result.Errors);

            return user;
        }

        /// <summary>
        /// Gets all the external login schemes, configured for authentication.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The collection of <see cref="ExternalScheme"/>.</returns>
        public virtual async Task<IEnumerable<ExternalScheme>> GetExternalSchemesAsync(CancellationToken cancellationToken = default)
        {
            var schemes = await this.SignInManager
                .GetExternalAuthenticationSchemesAsync();

            return schemes
                .Select(x => new ExternalScheme
                {
                    Name = x.Name,
                    DisplayName = x.DisplayName
                });
        }

        private void ThrowErrors(IEnumerable<IdentityError> errors)
        {
            if (errors == null) 
                throw new ArgumentNullException(nameof(errors));
            
            var exceptions = errors
                .Select(x => new InvalidOperationException($"{x.Code}: {x.Description}"));

            throw new AggregateException(exceptions);
        }
    }
}