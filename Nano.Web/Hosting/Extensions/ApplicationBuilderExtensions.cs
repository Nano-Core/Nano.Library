using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
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
        /// Adds ssl middleware to the <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseSsl(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();
                    
            if (webOptions.Hosting.UseSsl)
            {
                applicationBuilder
                    .UseRewriter(new RewriteOptions().AddRedirectToHttps());
            }
            
            return applicationBuilder;
        }

        /// <summary>
        /// Adds Hsts middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHsts(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();
                    
            if (webOptions.Hosting.UseHsts)
            {
                applicationBuilder
                    .UseHsts(x => x
                        .MaxAge(7)
                        .IncludeSubdomains()
                        .UpgradeInsecureRequests());
            }
            
            return applicationBuilder;
        }

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

            if (webOptions.Hosting.UseXRobotsTag)
            {
                applicationBuilder
                    .UseXRobotsTag(x => x
                        .NoIndex()
                        .NoFollow());
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

            if (webOptions.Hosting.XXssProtectionPolicy != XXssProtectionPolicy.Disabled)
            {
                applicationBuilder
                    .UseXXssProtection(x =>
                    {
                        switch (webOptions.Hosting.XXssProtectionPolicy)
                        {
                            case XXssProtectionPolicy.FilterEnabled:
                                x.Enabled();
                                break;

                            case XXssProtectionPolicy.FilterDisabled:
                                x.Disabled();
                                break;
                        }
                    });
            }

            return applicationBuilder;
        }
        
        /// <summary>
        /// Adds rerdirect valiation middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseRedirectValidationPolicies(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.UseRedirectValidation)
            {
                applicationBuilder
                    .UseRedirectValidation(x =>
                        x.AllowSameHostRedirectsToHttps());
            }
            
            return applicationBuilder;
        }
    }
}