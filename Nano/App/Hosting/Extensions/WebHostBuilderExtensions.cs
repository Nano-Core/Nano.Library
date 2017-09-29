using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Nano.App.Data.Extensions;
using Nano.App.Logging.Extensions;
using Nano.App.Logging.Providers.Interfaces;
using Serilog;

namespace Nano.App.Hosting.Extensions
{
    /// <summary>
    /// Web Host Builder Extensions.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Configures logging for the <typeparamref name="TLogging"/>.
        /// </summary>
        /// <param name="webHostBuilder">The <see cref="IWebHostBuilder"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseLogging<TLogging>(this IWebHostBuilder webHostBuilder)
            where TLogging : ILoggingProvider, new()
        {
            if (webHostBuilder == null)
                throw new ArgumentNullException(nameof(webHostBuilder));

            return webHostBuilder
                .ConfigureServices(x =>
                {
                    x.AddLogging<TLogging>();
                })
                .UseSerilog();
        }

        /// <summary>
        /// Configures data context for the <typeparamref name="TContext"/>.
        /// </summary>
        /// <param name="webHostBuilder">The <see cref="IWebHostBuilder"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseDataContext<TContext>(this IWebHostBuilder webHostBuilder)
            where TContext : DbContext
        {
            if (webHostBuilder == null)
                throw new ArgumentNullException(nameof(webHostBuilder));

            return webHostBuilder
                .ConfigureServices(x =>
                {
                    x.AddDataContext<TContext>();
                });
        }
    }
}