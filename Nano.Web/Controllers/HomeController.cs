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
using Nano.Web.Hosting.Extensions;

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
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public virtual IActionResult Index()
        {
            if (this.Request.IsContentTypeHtml())
                return this.View();

            return this.Ok();
        }

        /// <summary>
        /// Error action, returning an Ok (success) response containing the error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>The error.</returns>
        /// <response code="200">Success.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [Route("error")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.OK)]
        public virtual IActionResult Error([FromBody][Required]Error error)
        {
            if (this.Request.IsContentTypeHtml())
                return this.View(error);

            return this.Ok(error);
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
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
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

            if (this.Request.IsContentTypeHtml())
            {
                if (returnUrl == null)
                {
                    if (this.Request.IsContentTypeHtml())
                        return this.RedirectToAction("Index");

                    return Ok();
                }

                return this.LocalRedirect(returnUrl);
            }

            return this.Ok();
        }
    }
}