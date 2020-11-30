using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Security.Const;
using Nano.Security.Extensions;
using Nano.Web.Const;
using Nano.Web.Models;
using Vivet.AspNetCore.RequestTimeZone;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Home Controller.
    /// Contains method for handling application level operations.
    /// </summary>
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.READER + "," + BuiltInUserRoles.GUEST)]
    public class HomeController : BaseController
    {
        /// <summary>
        /// Services.
        /// </summary>
        protected virtual IServiceCollection Services { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        public HomeController(ILogger logger, IServiceCollection services)
            : base(logger)
        {
            this.Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Ping.
        /// </summary>
        /// <returns>Void.</returns>
        /// <response code="200">OK.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("ping")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual IActionResult Ping()
        {
            return this.Ok();
        }

        /// <summary>
        /// Get information about the current user.
        /// </summary>
        /// <returns>The user information.</returns>
        /// <response code="200">OK.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("user")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(IEnumerable<UserInformation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual IActionResult GetUser()
        {
            var id = this.HttpContext.GetJwtUserId();
            var appId = this.HttpContext.GetJwtAppId();
            var name = this.HttpContext.GetJwtUserName();
            var email = this.HttpContext.GetJwtUserEmail();
            var jwtToken = this.HttpContext.GetJwtToken();
            var timezone = this.HttpContext.GetUserTimeZone();
            var culture = CultureInfo.CurrentCulture.Name;

            var userInfo = new UserInformation
            {
                Id = id,
                AppId = appId,
                Name = name,
                Email = email,
                JwtToken = jwtToken,
                TimeZone = timezone?.Id,
                Culture = culture
            };

            return this.Ok(userInfo);
        }

        /// <summary>
        /// Get Services.
        /// The services dependencies registerd during application start-up.
        /// </summary>
        /// <param name="filter">The filter on service or implementation name.</param>
        /// <returns>The services.</returns>
        /// <response code="200">OK.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("services")]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(IEnumerable<ServiceDependency>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual IActionResult GetServices([FromQuery]string filter = "")
        {
            var serviceDescriptors = this.Services
                .Where(x => 
                    x.ServiceType?.FullName != null && x.ServiceType?.FullName?.IndexOf(filter) >= 0 ||
                    x.ImplementationType?.FullName != null && x.ImplementationType?.FullName?.IndexOf(filter) >= 0)
                .Select(x => new ServiceDependency
                {
                    Service = x.ServiceType.FullName, 
                    Implementation = x.ImplementationType?.FullName, 
                    LifeTime = x.Lifetime
                })
                .OrderBy(x => x.Service);

            return this.Ok(serviceDescriptors);
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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