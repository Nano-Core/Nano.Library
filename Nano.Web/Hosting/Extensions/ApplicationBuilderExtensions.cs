using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nano.Web.Hosting.Enums;
using NWebsec.AspNetCore.Mvc;
using ReferrerPolicy = NWebsec.Core.Common.HttpHeaders.ReferrerPolicy;

namespace Nano.Web.Hosting.Extensions
{
    /// <summary>
    /// Application Builder Extensions.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds no cache middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseNoCache(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.UseNoCache)
            {
                applicationBuilder
                    .UseNoCacheHttpHeaders();
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds x-Robots tag middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseRobotsTag(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.Robots.IsEnabled)
            {
                var xRobot = webOptions.Hosting.Robots;

                applicationBuilder
                    .UseXRobotsTag(x =>
                    {
                        if (xRobot.UseNoIndex)
                            x.NoIndex();

                        if (xRobot.UseNoFollow)
                            x.NoFollow();

                        if (xRobot.UseNoSnippet)
                            x.NoSnippet();

                        if (xRobot.UseNoArchive)
                            x.NoArchive();

                        if (xRobot.UseNoOdp)
                            x.NoOdp();

                        if (xRobot.UseNoTranslate)
                            x.NoTranslate();

                        if (xRobot.UseNoImageIndex)
                            x.NoImageIndex();
                    });
            }
            
            return applicationBuilder;
        }

        /// <summary>
        /// Adds rerdirect valiation middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpsRedirect(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.UseHttpsRedirect)
            {
                applicationBuilder
                    .UseRedirectValidation(x =>
                    {
                        x.AllowedDestinations();
                        x.AllowSameHostRedirectsToHttps(webOptions.Hosting.PortsHttps);
                    });
            }
            
            return applicationBuilder;
        }

        /// <summary>
        /// Adds x-download options middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseDownloadOptions(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.UseXDownloadOptions)
            {
                applicationBuilder
                    .UseXDownloadOptions();
            }
            
            return applicationBuilder;
        }

        /// <summary>
        /// Adds x-content-type options middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseContentTypeOptions(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.UseXContentTypeOptions)
            {
                applicationBuilder
                    .UseXContentTypeOptions();
            }
            
            return applicationBuilder;
        }

        /// <summary>
        /// Adds referrer policy middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseReferrerPolicies(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.ReferrerPolicy != ReferrerPolicy.Disabled)
            {
                applicationBuilder
                    .UseReferrerPolicy(x =>
                    {
                        switch (webOptions.Hosting.ReferrerPolicy)
                        {
                            case ReferrerPolicy.NoReferrer:
                                x.NoReferrer();
                                break;

                            case ReferrerPolicy.NoReferrerWhenDowngrade:
                                x.NoReferrerWhenDowngrade();
                                break;

                            case ReferrerPolicy.SameOrigin:
                                x.SameOrigin();
                                break;

                            case ReferrerPolicy.Origin:
                                x.Origin();
                                break;
                            
                            case ReferrerPolicy.StrictOrigin:
                                x.StrictOrigin();
                                break;
                            
                            case ReferrerPolicy.OriginWhenCrossOrigin:
                                x.OriginWhenCrossOrigin();
                                break;
                            
                            case ReferrerPolicy.StrictOriginWhenCrossOrigin:
                                x.StrictOriginWhenCrossOrigin();
                                break;

                            case ReferrerPolicy.UnsafeUrl:
                                x.UnsafeUrl();
                                break;
                        }
                    });
            }
            
            return applicationBuilder;
        }

        /// <summary>
        /// Adds X-frame options policy middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseXFrameOptionsPolicies(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.XFrameOptionsPolicy != XFrameOptionsPolicy.Disabled)
            {
                applicationBuilder
                    .UseXfo(x =>
                    {
                        switch (webOptions.Hosting.XFrameOptionsPolicy)
                        {
                            case XFrameOptionsPolicy.Deny:
                                x.Deny();
                                break;

                            case XFrameOptionsPolicy.SameOrigin:
                                x.SameOrigin();
                                break;
                        }
                    });
            }
            
            return applicationBuilder;
        }

        /// <summary>
        /// Adds XXss protection policy middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseXXssProtectionPolicies(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.XXssProtectionPolicy != XXssProtectionPolicyBlockMode.Disabled)
            {
                applicationBuilder
                    .UseXXssProtection(x =>
                    {
                        switch (webOptions.Hosting.XXssProtectionPolicy)
                        {
                            case XXssProtectionPolicyBlockMode.FilterDisabled:
                                x.Disabled();
                                break;

                            case XXssProtectionPolicyBlockMode.FilterEnabled:
                                x.Enabled();
                                break;

                            case XXssProtectionPolicyBlockMode.FilterEnabledBlockMode:
                                x.EnabledWithBlockMode();
                                break;
                        }
                    });
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds Hsts middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseStrictTransportSecurity(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();
            var hsts = webOptions.Hosting.Hsts;
                    
            if (hsts.IsEnabled)
            {
                var maxAge = hsts.MaxAge;

                applicationBuilder
                    .UseHsts(x =>
                    {
                        if (hsts.IncludeSubdomains)
                        {
                            x.IncludeSubdomains();

                            if (maxAge.HasValue)
                            {
                                const int MAX_WEEKS = 18;
                                var weeks = maxAge.Value.TotalDays / 7;  

                                if (hsts.UsePreload && weeks >= MAX_WEEKS)
                                    x.Preload();
                            }
                        }

                        if (maxAge.HasValue)
                        {
                            x.MaxAge(maxAge.Value.Days, maxAge.Value.Hours, maxAge.Value.Minutes, maxAge.Value.Seconds);
                        }
                    });
            }
            
            return applicationBuilder;
        }
    }
}