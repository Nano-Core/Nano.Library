using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nano.Models;
using Nano.Models.Types;
using Nano.Web.Controllers.Extensions;

namespace Nano.Web.Controllers
{
    // TODO: Security

    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    public class AuthController : Controller
    {
        /// <summary>
        /// Gets all models. 
        /// Filtered by query model parameters (pagination and ordering).
        /// </summary>
        /// <param name="authenticationCredential">The authentication credential.</param>
        /// <returns>A jwt token.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="401">Unauthorized. Invalid authentication credential.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual IActionResult Authenticate([FromBody][Required]AuthenticationCredential authenticationCredential)
        {
            var success = this.Validate(authenticationCredential);

            if (!success)
                return this.Unauthorized();

            var token = new
            {
                Token = this.CreateToken()
            };

            return this.Ok(token);
        }

        /// <summary>
        /// Creates the jwt token.
        /// </summary>
        /// <returns>The string token.</returns>
        protected internal virtual string CreateToken()
        {
            var key = "key";
            var issuer = "issuer";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(issuer, issuer, null, DateTime.UtcNow.AddMinutes(30), null, signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        /// <summary>
        /// Authenticates using the passed <see cref="AuthenticationCredential"/>.
        /// </summary>
        /// <param name="authenticationCredential">The <see cref="AuthenticationCredential"/>.</param>
        /// <returns>whether the authentication succeeded or not.</returns>
        protected internal virtual bool Validate(AuthenticationCredential authenticationCredential)
        {
            if (authenticationCredential.Username == "mario" && authenticationCredential.Password == "secret")
                return true;

            return false;
        }
    }
}