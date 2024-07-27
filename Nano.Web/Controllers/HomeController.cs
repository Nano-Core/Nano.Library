﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Models;
using Nano.Models.Const;
using Nano.Security.Const;
using Nano.Security.Extensions;
using Nano.Web.Models;
using Vivet.AspNetCore.RequestTimeZone;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;

namespace Nano.Web.Controllers;

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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<UserInformation>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual IActionResult GetUser()
    {
        var id = this.HttpContext.GetJwtUserId();
        var appId = this.HttpContext.GetJwtAppId();
        var username = this.HttpContext.GetJwtUserName();
        var email = this.HttpContext.GetJwtUserEmail();
        var jwtToken = this.HttpContext.GetJwtToken();
        var timezone = this.HttpContext.GetUserTimeZone();
        var culture = CultureInfo.CurrentCulture.Name;

        var userInfo = new UserInformation
        {
            Id = id,
            AppId = appId,
            Name = username,
            Email = email,
            JwtToken = jwtToken,
            TimeZone = timezone?.Id,
            Culture = culture
        };

        return this.Ok(userInfo);
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
    [Consumes(HttpContentType.JSON)]
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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual IActionResult SetTimeZone([FromQuery][Required]string name, CancellationToken cancellationToken = default)
    {
        var cookieName = RequestTimeZoneCookieProvider.defaultCookieName;
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