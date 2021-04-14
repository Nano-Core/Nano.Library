using System;
using System.Globalization;
using System.Linq;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nano.App;
using Nano.Config;
using Nano.Web.Enums;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Middleware;
using NWebsec.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerUI;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;
using ReferrerPolicy = NWebsec.AspNetCore.Mvc.ReferrerPolicy;

namespace Nano.Web.Extensions
{
    /// <summary>
    /// Application Builder Extensions.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds health checks middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHealthChecks(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.HealthCheck.UseHealthCheck)
            {
                applicationBuilder
                    .UseHealthChecks("/healthz", new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        AllowCachingResponses = true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });

                if (webOptions.Hosting.HealthCheck.UseHealthCheckUI)
                {
                    applicationBuilder
                        .UseHealthChecksUI(x =>
                        {
                            x.UIPath = "/healthz-ui";
                            x.ApiPath = "/healthz-api";
                            x.ResourcesPath = "/healthz-rex";
                            x.WebhookPath = "/healthz-hooks";
                            x.UseRelativeApiPath = true;
                            x.UseRelativeResourcesPath = true;
                            x.UseRelativeWebhookPath = true;
                        });
                }
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds exception handling middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            applicationBuilder
                .UseMiddleware<ExceptionHandlingMiddleware>();

            return applicationBuilder;
        }
        
        /// <summary>
        /// Adds options action middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpRequestOptions(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            applicationBuilder
                .UseMiddleware<HttpRequestOptionsMiddleware>();

            return applicationBuilder;
        }
 
        /// <summary>
        /// Adds request identifier middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpRequestIdentifier(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            applicationBuilder
                .UseMiddleware<HttpRequestIdentifierMiddleware>();

            return applicationBuilder;
        }

        /// <summary>
        /// Adds documentation middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpDocumentataion(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var appOptions = services.GetService<AppOptions>() ?? new AppOptions();

            applicationBuilder
                .UseSwagger(x =>
                {
                    x.RouteTemplate = "docs/{documentName}/swagger.json";
                })
                .UseSwaggerUI(x =>
                {
                    x.EnableFilter();
                    x.EnableDeepLinking();
                    x.EnableValidator(null);
                    x.ShowExtensions();
                    x.DisplayOperationId();
                    x.DisplayRequestDuration();
                    x.MaxDisplayedTags(-1);
                    x.DefaultModelExpandDepth(2);
                    x.DefaultModelsExpandDepth(1);
                    x.DefaultModelRendering(ModelRendering.Example);
                    x.DocExpansion(DocExpansion.None);

                    x.RoutePrefix = "docs";
                    x.DocumentTitle = $"Nano - {appOptions.Name} Docs v{appOptions.Version} ({ConfigManager.Environment})";
                    x.SwaggerEndpoint($"{appOptions.Version}/swagger.json", $"Nano - {appOptions.Name} v{appOptions.Version} ({ConfigManager.Environment})");
                });

            return applicationBuilder;
        }

        /// <summary>
        /// Adds localization middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpLocalization(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var appOptions = services.GetService<AppOptions>() ?? new AppOptions();

            applicationBuilder
                .UseRequestLocalization(x =>
                {
                    x.DefaultRequestCulture = new RequestCulture(appOptions.Cultures.Default);
                    x.SupportedCultures = appOptions.Cultures.Supported.Select(y => new CultureInfo(y)).ToArray();
                    x.SupportedUICultures = appOptions.Cultures.Supported.Select(y => new CultureInfo(y)).ToArray();
                });

            return applicationBuilder;
        }

        /// <summary>
        /// Adds timezone middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpRequestTimeZone(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            applicationBuilder
                .UseRequestTimeZone();

            return applicationBuilder;
        }

        /// <summary>
        /// Adds the <see cref="IHttpContextAccessor"/> middleware, and initializes the current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpContextAccessor(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var httpContextAccessor = applicationBuilder.ApplicationServices
                .GetRequiredService<IHttpContextAccessor>();
            
            HttpContextAccess.Configure(httpContextAccessor);

            return applicationBuilder;
        }

        /// <summary>
        /// Adds http session middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpSession(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.Session.IsEnabled)
            {
                applicationBuilder
                    .UseSession(new SessionOptions
                    {
                        IdleTimeout = webOptions.Hosting.Session.Timeout
                    });
            }
            
            return applicationBuilder;
        }
                
        /// <summary>
        /// Adds response caching middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpResponseCaching(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.Cache.IsEnabled)
            {
                applicationBuilder
                    .UseResponseCaching()
                    .Use(async (context, next) =>
                    {
                        context.Response.GetTypedHeaders().CacheControl =
                            new CacheControlHeaderValue
                            {
                                Public = true,
                                MaxAge = webOptions.Hosting.Cache.MaxAge
                            };
                        
                        context.Response.Headers[HeaderNames.Vary] = 
                            new[]
                            {
                                HeaderNames.Authorization,
                                HeaderNames.AcceptEncoding, 
                                HeaderNames.AcceptLanguage,
                                RequestTimeZoneHeaderProvider.Headerkey
                            };

                        var responseCachingFeature = context.Features
                            .Get<IResponseCachingFeature>();

                        if (responseCachingFeature != null)
                        {
                            responseCachingFeature.VaryByQueryKeys = new[]
                            {
                                "*"
                            };
                        }

                        await next();
                    });
            }
            else
            {
                applicationBuilder
                    .UseNoCacheHttpHeaders();
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds response compresssion middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpResponseCompression(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.UseResponseCompression)
            {
                applicationBuilder
                    .UseResponseCompression();
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds no cache middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpXForwardedHeaders(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.UseForwardedHeaders)
            {
                applicationBuilder
                    .UseForwardedHeaders(); 
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds x-Robots tag middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpXRobotsTagHeaders(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();
            var xRobotTags = webOptions.Hosting.Robots;

            if (xRobotTags.IsEnabled)
            {
                applicationBuilder
                    .UseXRobotsTag(x =>
                    {
                        if (xRobotTags.UseNoIndex)
                            x.NoIndex();

                        if (xRobotTags.UseNoFollow)
                            x.NoFollow();

                        if (xRobotTags.UseNoSnippet)
                            x.NoSnippet();

                        if (xRobotTags.UseNoArchive)
                            x.NoArchive();

                        if (xRobotTags.UseNoOdp)
                            x.NoOdp();

                        if (xRobotTags.UseNoTranslate)
                            x.NoTranslate();

                        if (xRobotTags.UseNoImageIndex)
                            x.NoImageIndex();
                    });
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds referrer policy middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpReferrerPolicyHeader(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.ReferrerPolicyHeader != ReferrerPolicy.Disabled)
            {
                applicationBuilder
                    .UseReferrerPolicy(x =>
                    {
                        switch (webOptions.Hosting.ReferrerPolicyHeader)
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

                            case ReferrerPolicy.Disabled:
                                break;
                            
                            default:
                                throw new ArgumentOutOfRangeException();
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
        internal static IApplicationBuilder UseHttpXFrameOptionsPolicyHeader(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.FrameOptionsPolicyHeader != XFrameOptionsPolicy.Disabled)
            {
                applicationBuilder
                    .UseXfo(x =>
                    {
                        switch (webOptions.Hosting.FrameOptionsPolicyHeader)
                        {
                            case XFrameOptionsPolicy.Deny:
                                x.Deny();
                                break;

                            case XFrameOptionsPolicy.SameOrigin:
                                x.SameOrigin();
                                break;

                            case XFrameOptionsPolicy.Disabled:
                                break;
                            
                            default:
                                throw new ArgumentOutOfRangeException();
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
        internal static IApplicationBuilder UseHttpXXssProtectionPolicyHeader(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.XssProtectionPolicyHeader != XXssProtectionPolicyBlockMode.Disabled)
            {
                applicationBuilder
                    .UseXXssProtection(x =>
                    {
                        switch (webOptions.Hosting.XssProtectionPolicyHeader)
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

                            case XXssProtectionPolicyBlockMode.Disabled:
                                break;
                            
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    });
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds https content type options policy header middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseXHttpContentTypeOptionsPolicyHeader(this IApplicationBuilder applicationBuilder)
        {
            // https://docs.nwebsec.com/en/latest/nwebsec/Configuring-cto.html

            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            if (webOptions.Hosting.UseContentTypeOptions)
            {
                applicationBuilder
                    .UseXContentTypeOptions();
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds Hsts middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpStrictTransportSecurityPolicyHeader(this IApplicationBuilder applicationBuilder)
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

        /// <summary>
        /// Adds Content Security Policy (CSP) middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseContentSecurityPolicyHeader(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null) 
                throw new ArgumentNullException(nameof(applicationBuilder));

            // TODO: CSP - Missing from NWebSec
            // navigate-to,
            // prefetch-src,
            // report-to,
            // require-trusted-types-for,
            // trusted-types,
            // require-sri-for,
            // script-src-attr,
            // script-src-elem,
            // style-src-attr,
            // style-src-elem,
            // Sandbox (allow-downloads-without-user-activation, allow-storage-access-by-user-activation, allow-top-navigation-by-user-activation)

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();
            var csp = webOptions.Hosting.Csp;

            if (csp.BlockAllMixedContent)
            {
                applicationBuilder
                    .UseCsp(x =>
                        x.BlockAllMixedContent());
            }

            if (csp.UpgradeInsecureRequests)
            {
                applicationBuilder
                    .UseCsp(x =>
                    {
                        if (webOptions.Hosting.PortsHttps.Any())
                        {
                            x.UpgradeInsecureRequests(webOptions.Hosting.PortsHttps.FirstOrDefault());
                        }
                        else
                        {
                            x.UpgradeInsecureRequests();
                        }
                    });

                applicationBuilder
                    .UseRedirectValidation(x =>
                    {
                        x.AllowSameHostRedirectsToHttps(webOptions.Hosting.PortsHttps);
                    });
            }

            applicationBuilder
                .UseCspDefaults(csp.Defaults)
                .UseCspScripts(csp.Scripts)
                .UseCspStyles(csp.Styles)
                .UseCspObjects(csp.Objects)
                .UseCspImages(csp.Images)
                .UseCspMedia(csp.Media)
                .UseCspFrames(csp.Frames)
                .UseCspFrameAncestors(csp.FrameAncestors)
                .UseCspFonts(csp.Fonts)
                .UseCspConnections(csp.Connections)
                .UseCspBaseUris(csp.BaseUris)
                .UseCspChildren(csp.Children)
                .UseCspForms(csp.Forms)
                .UseCspManifests(csp.Manifests)
                .UseCspWorkers(csp.Workers)
                .UseCspSandbox(csp.Sandbox);

            if (csp.PluginTypes.Any())
            {
                applicationBuilder
                    .UseCsp(x => 
                        x.PluginTypes(y => 
                            y.MediaTypes(csp.PluginTypes)));
            }

            if (csp.ReportUris.Any())
            {
                applicationBuilder
                    .UseCsp(x =>
                        x.ReportUris(y => 
                            y.Uris()));
            }

            return applicationBuilder;
        }

        /// <summary>
        /// Adds CORS middleware to the <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHttpCorsPolicy(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

            applicationBuilder
                .UseCors(x =>
                {
                    if (webOptions.Hosting.AllowedOrigins.Any())
                    {
                        x.WithOrigins(webOptions.Hosting.AllowedOrigins);
                        x.SetIsOriginAllowedToAllowWildcardSubdomains();
                    }
                    else
                    {
                        x.AllowAnyOrigin();
                    }

                    x.AllowAnyHeader();
                    x.AllowAnyMethod();
                    x.WithExposedHeaders("RequestId", "TZ");
                });
            
            return applicationBuilder;
        }

        private static IApplicationBuilder UseCspDefaults(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null) 
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.DefaultSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.DefaultSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });
            
            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspScripts(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirectiveScripts cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.ScriptSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.ScriptSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            if (cspDirective.IsUnsafeEval)
                            {
                                y = y.UnsafeEval();
                            }

                            if (cspDirective.IsUnsafeInline)
                            {
                                y = y.UnsafeInline();
                            }

                            if (cspDirective.StrictDynamic)
                            {
                                y = y.StrictDynamic();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspStyles(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirectiveStyles cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.StyleSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.StyleSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            if (cspDirective.IsUnsafeInline)
                            {
                                y = y.UnsafeInline();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspObjects(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.ObjectSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.ObjectSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspImages(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.ImageSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.ImageSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspMedia(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.MediaSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.MediaSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspFrames(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.FrameSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.FrameSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspFrameAncestors(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.FrameAncestors(y =>
                            y.None());
                    }
                    else
                    {
                        x.FrameAncestors(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspFonts(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.FontSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.FontSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspConnections(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.ConnectSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.ConnectSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspBaseUris(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.BaseUris(y =>
                            y.None());
                    }
                    else
                    {
                        x.BaseUris(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspChildren(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.ChildSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.ChildSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspForms(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.FormActions(y =>
                            y.None());
                    }
                    else
                    {
                        x.FormActions(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspManifests(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.ManifestSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.ManifestSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspWorkers(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspDirective == null)
                throw new ArgumentNullException(nameof(cspDirective));

            if (!cspDirective.IsEnabled)
                return applicationBuilder;

            applicationBuilder
                .UseCsp(x =>
                {
                    if (cspDirective.IsNone)
                    {
                        x.WorkerSources(y =>
                            y.None());
                    }
                    else
                    {
                        x.WorkerSources(y =>
                        {
                            if (cspDirective.IsSelf)
                            {
                                y = y.Self();
                            }

                            y.CustomSources(cspDirective.Sources);
                        });
                    }
                });

            return applicationBuilder;
        }
        private static IApplicationBuilder UseCspSandbox(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspSandbox cspSandbox)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (cspSandbox == null)
                throw new ArgumentNullException(nameof(cspSandbox));

            applicationBuilder
                .UseCsp(x =>
                    x.Sandbox(y =>
                    {
                        if (cspSandbox.AllowForms)
                        {
                            y = y.AllowForms();
                        }

                        if (cspSandbox.AllowModals)
                        {
                            y = y.AllowModals();
                        }

                        if (cspSandbox.AllowOrientationLock)
                        {
                            y = y.AllowOrientationLock();
                        }

                        if (cspSandbox.AllowPointerLock)
                        {
                            y = y.AllowPointerLock();
                        }

                        if (cspSandbox.AllowPopups)
                        {
                            y = y.AllowPopups();
                        }

                        if (cspSandbox.AllowPopupsToEscapeSandbox)
                        {
                            y = y.AllowPopupsToEscapeSandbox();
                        }

                        if (cspSandbox.AllowPresentation)
                        {
                            y = y.AllowPresentation();
                        }

                        if (cspSandbox.AllowSameOrigin)
                        {
                            y = y.AllowSameOrigin();
                        }

                        if (cspSandbox.AllowScripts)
                        {
                            y = y.AllowScripts();
                        }

                        if (cspSandbox.AllowTopNavigation)
                        {
                            y.AllowTopNavigation();
                        }
                    }));

            return applicationBuilder;
        }
    }
}