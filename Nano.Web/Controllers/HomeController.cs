using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Nano.Models;
using Nano.Web.Controllers.Extensions;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Home Controller.
    /// Contains method for handling application level operations.
    /// </summary>
    [Authorize]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Gets Ok response.
        /// </summary>
        /// <returns>Nothing (void).</returns>
        /// <response code="200">Success.</response>
        [HttpGet]
        [Route("index")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public virtual IActionResult Index()
        {
            if (this.Response.IsContentTypeHtml())
                return this.View();

            return this.Ok();
        }

        /// <summary>
        /// Ping the application. Is it alive.
        /// </summary>
        /// <returns>Nothing (void).</returns>
        /// <response code="200">Success.</response>
        [HttpGet]
        [HttpPost]
        [Route("ping")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public virtual IActionResult Ping()
        {
            if (this.Response.IsContentTypeHtml())
                return this.View();

            return Ok();
        }

        /// <summary>
        /// Gets the http options.
        /// </summary>
        /// <returns>The http options.</returns>
        /// <response code="200">Success.</response>
        [HttpOptions]
        [Route("options")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public virtual IActionResult Options()
        {
            // FEATURE: Http Options: https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/OPTIONS

            if (this.Response.IsContentTypeHtml())
                return this.View();

            return Ok(); 
        }

        /// <summary>
        /// Gets the supported versions of the application.
        /// </summary>
        /// <returns>The supported versions.</returns>
        /// <response code="200">Success.</response>
        [HttpGet]
        [Route("versions")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public virtual IActionResult Versions()
        {
            // FEATURE: Http supported Versions.

            if (this.Response.IsContentTypeHtml())
                return this.View();

            return this.Ok(); 
        }

        /// <summary>
        /// Gets the error occured.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>The error.</returns>
        /// <response code="200">Success.</response>
        [HttpGet]
        [HttpPost]
        [Route("error")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.OK)]
        public virtual IActionResult Error([FromQuery][FromBody][Required]Error error)
        {
            if (this.Response.IsContentTypeHtml())
                return this.View(error);

            return this.Ok(error);
        }

        /// <summary>
        /// Sets the language used for future requests.
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
        public virtual IActionResult SetLanguage([FromQuery][FromHeader(Name = "Accept-Language")][Required]string code, [FromQuery]string returnUrl, CancellationToken cancellationToken = new CancellationToken())
        {
            // FEATURE: Http Localization, how to use IRequestCultureProviders? HomeController.SetLanguage?

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
                    if (this.Response.IsContentTypeHtml())
                        return this.RedirectToAction("Index");

                    return Ok();
                }

                return this.LocalRedirect(returnUrl);
            }

            return this.Ok();
        }
    }
}