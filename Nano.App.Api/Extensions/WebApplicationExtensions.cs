using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Api.Config;
using Nano.Eventing.Abstractions.Extensions;
using System;
using Nano.Data.Abstractions.Eventing.Extensions;
using Nano.Data.Abstractions.Extensions;

namespace Nano.App.Api.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication ConfigureNanoApiApplication(this WebApplication webApplication, ApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(webApplication);
        ArgumentNullException.ThrowIfNull(options);

        webApplication
            .UseNanoExceptionHandling()
            .UseNanoHttpsRedirection(options.Hosting.Http, options.Hosting.Https)
            .UseNanoHttpXForwardedHeaders(options.HttpPolicyHeaders.ForwardedHeaders)
            .UseNanoHttpXRobotsTagHeaders(options.HttpPolicyHeaders.Robots)
            .UseNanoHttpXFrameOptionsPolicyHeader(options.HttpPolicyHeaders.FrameOptions)
            .UseNanoHttpXXssProtectionPolicyHeader(options.HttpPolicyHeaders.XssProtection)
            .UseNanoHttpContentTypeOptionsPolicyHeader(options.HttpPolicyHeaders.ContentType)
            .UseNanoHttpReferrerPolicyHeader(options.HttpPolicyHeaders.ReferrerPolicy)
            .UseNanoHttpStrictTransportSecurityPolicyHeader(options.HttpPolicyHeaders.Hsts)
            .UseNanoHttpContentSecurityPolicyHeader(options.HttpPolicyHeaders.Csp)
            .UseStaticFiles()
            .UseCookiePolicy()
            .UseRouting()
            .UseNanoHttpCorsPolicy(options.HttpPolicyHeaders.Cors)
            .UseAuthentication()
            .UseAuthorization()
            .UseNanoSession(options.Session)
            .UseNanoRequestIdentifier()
            .UseNanoApiVersion()
            .UseNanoRequestVirusScan(options.VirusScan)
            .UseNanoRequestLocalization(options.Localization)
            .UseNanoRequestTimeZone(options.TimeZone)
            .UseNanoResponseCompression(options.ResponseCompression)
            .UseNanoResponseCaching(options.ResponseCache)
            .UseNanoDocumentataion(webApplication.Environment, options.Version, options.Documentation)
            .UseNanoHealthChecks(webApplication.Environment, options.HealthCheck);

        webApplication
            .MapControllers();

        using var serviceScope = webApplication.Services
            .CreateScope();

        serviceScope
            .UseNanoEndpoints(webApplication, options)
            .UseEventHandlers(webApplication.Services)
            .UseEntityEventing(webApplication.Services)
            .UseNanoDbMigrations();

        return webApplication;
    }
}