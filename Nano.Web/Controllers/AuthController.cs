using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nano.Models;
using Nano.Security;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Extensions;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Auth Controller.
    /// </summary>
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        /// <summary>
        /// Security Options.
        /// </summary>
        protected virtual SecurityOptions SecurityOptions { get; }

        /// <summary>
        /// User Manager.
        /// </summary>
        protected virtual UserManager<IdentityUser> UserManager { get; }

        /// <summary>
        /// Sign In Manager.
        /// </summary>
        protected virtual SignInManager<IdentityUser> SignInManager { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
        /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
        /// <param name="securityOptions">The <see cref="SecurityOptions"/>.</param>
        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, SecurityOptions securityOptions)
        {
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            if (signInManager == null)
                throw new ArgumentNullException(nameof(signInManager));

            if (securityOptions == null)
                throw new ArgumentNullException(nameof(securityOptions));

            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.SecurityOptions = securityOptions;
        }

        /// <summary>
        /// Gets the view for access denied.
        /// </summary>
        /// <returns>The view.</returns>
        [HttpGet]
        [Route("forbidden")]
        [Produces(HttpContentType.HTML)]
        public virtual IActionResult Forbidden()
        {
            return View();
        }

        /// <summary>
        /// The user authenticates and on success recieves a jwt token for use with auhtorization.
        /// </summary>
        /// <param name="login">The login model.</param>
        /// <param name="returnUrl"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A jwt token.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("login")]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Login([FromBody][Required]Login login, string returnUrl = null, CancellationToken cancellationToken = default)
        {
            var result = await this.SignInManager
                .PasswordSignInAsync(login.Username, login.Password, false, false);

            if (!result.Succeeded)
                return this.Unauthorized();

            var user = this.UserManager.Users
                .SingleOrDefault(x => x.UserName == login.Username);

            var token = await this.GenerateJwtToken(user);

            if (this.Request.IsContentTypeHtml())
                return this.LocalRedirect(returnUrl);

            return this.Ok(token);
        }

        /// <summary>
        /// The user is logged out, and the token is invalidated.
        /// Usually, it's not needed to call this method, unless having a specific reason for invalidating a token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="500">Error occurred.</response>
        [HttpGet]
        [HttpPost]
        [Route("logout")]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            await this.SignInManager.SignOutAsync();

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }

        /// <summary>
        /// Generates the Jwt token.
        /// </summary>
        /// <param name="user">The <see cref="IdentityUser"/>.</param>
        /// <returns>The token.</returns>
        protected virtual async Task<AccessToken> GenerateJwtToken(IdentityUser user)
        {
            var roles = await this.UserManager.GetRolesAsync(user);
            var userClaims = await this.UserManager.GetClaimsAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));

            var claims = new Collection<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var notBeforeAt = DateTime.UtcNow;
            var expireAt = DateTime.UtcNow.AddHours(this.SecurityOptions.Jwt.ExpirationInHours);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.SecurityOptions.Jwt.SecretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(this.SecurityOptions.Jwt.Issuer, this.SecurityOptions.Jwt.Issuer, claims, notBeforeAt, expireAt, signingCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AccessToken
            {
                Token = token,
                ExpireAt = expireAt
            };
        }
    }
}