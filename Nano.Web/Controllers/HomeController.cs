using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Nano.Models;
using Nano.Web.Hosting;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Home Controller.
    /// Contains method for handling application level operations.
    /// </summary>
    [Authorize(Roles = "administrator, service, writer, reader, guest")]
    public class HomeController : BaseController
    {
        /// <summary>
        /// Default action, returning an Ok (success) response.
        /// </summary>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet]
        [Route("index")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public virtual IActionResult Index()
        {
            return this.Ok();
        }

        /// <summary>
        /// Sets the language in a cookie, for use with following requests.
        /// </summary>
        /// <param name="code">The langauge code.</param>
        /// <param name="returnUrl">The return url (if any).</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothing (void).</returns>
        [HttpGet]
        [HttpPost]
        [Route("language")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.OK)]
        public virtual IActionResult SetLanguage([FromQuery][Required]string code, [FromQuery]string returnUrl, CancellationToken cancellationToken = default)
        {
            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(code));
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            };

            this.Response.Cookies
                .Append(cookieName, cookieValue, cookieOptions);

            return this.Ok();
        }
    }
}