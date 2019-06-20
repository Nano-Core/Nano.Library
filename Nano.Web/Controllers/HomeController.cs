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
using Vivet.AspNetCore.RequestTimeZone;

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
        /// Default action (ping).
        /// </summary>
        /// <returns>Void.</returns>
        /// <response code="200">OK.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("index")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual IActionResult Index()
        {
            return this.Ok();
        }

        /// <summary>
        /// Sets the language in a cookie, for use with following requests.
        /// </summary>
        /// <param name="code">The langauge code.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothing (void).</returns>
        /// <response code="200">OK.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("language")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual IActionResult SetLanguage([FromQuery][Required]string code, CancellationToken cancellationToken = default)
        {
            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(code));
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(14)
            };

            this.Response.Cookies
                .Append(cookieName, cookieValue, cookieOptions);

            return this.Ok();
        }
        
        /// <summary>
        /// Sets the timezone in a cookie, for use with following requests.
        /// </summary>
        /// <param name="name">The timezone name.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothing (void).</returns>
        /// <response code="200">OK.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("timezone")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual IActionResult SetTimeZone([FromQuery][Required]string name, CancellationToken cancellationToken = default)
        {
            var cookieName = RequestTimeZoneCookieProvider.DefaultCookieName;
            var cookieValue = RequestTimeZoneCookieProvider.MakeCookieValue(new RequestTimeZone(name));
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(14)
            };

            this.Response.Cookies
                .Append(cookieName, cookieValue, cookieOptions);

            this.Response.Headers[RequestTimeZoneHeaderProvider.Headerkey] = name;

            return this.Ok();
        }
    }
}