using System;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Console.Hosting.Interfaces;

namespace Nano.Console.Hosting.Extensions
{
    /// <summary>
        /// Console Host Builder Extensions.
        /// </summary>
        public static class ConsoleHostBuilderExtensions
    {
        /// <summary>
        /// Use the given configuration settings on the console host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to configure.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing settings to be used.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder UseConfiguration(this IConsoleHostBuilder hostBuilder, IConfiguration configuration)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            foreach (var setting in configuration.AsEnumerable())
            {
                hostBuilder.UseSetting(setting.Key, setting.Value);
            }

            return hostBuilder;
        }

        /// <summary>
        /// Set whether startup errors should be captured in the configuration settings of the console host.
        /// When enabled, startup exceptions will be caught and an error page will be returned. If disabled, startup exceptions will be propagated.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to configure.</param>
        /// <param name="captureStartupErrors"><c>true</c> to capture startup errors; otherwise <c>false</c>.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder CaptureStartupErrors(this IConsoleHostBuilder hostBuilder, bool captureStartupErrors)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            return hostBuilder
                .UseSetting(ConsoleHostDefaults.CaptureStartupErrorsKey, captureStartupErrors ? "true" : "false");
        }

        /// <summary>
        /// Specify the assembly containing the startup type to be used by the console host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to configure.</param>
        /// <param name="startupAssemblyName">The name of the assembly containing the startup type.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder UseStartup(this IConsoleHostBuilder hostBuilder, string startupAssemblyName)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            if (startupAssemblyName == null)
                throw new ArgumentNullException(nameof(startupAssemblyName));

            return hostBuilder
                .UseSetting(ConsoleHostDefaults.ApplicationKey, startupAssemblyName)
                .UseSetting(ConsoleHostDefaults.StartupAssemblyKey, startupAssemblyName);
        }

        /// <summary>
        /// Specify the startup type to be used by the console host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to configure.</param>
        /// <param name="startupType">The <see cref="Type"/> to be used.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder UseStartup(this IConsoleHostBuilder hostBuilder, Type startupType)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            if (startupType == null)
                throw new ArgumentNullException(nameof(startupType));

            var startupAssemblyName = startupType.GetTypeInfo().Assembly.GetName().Name;

            return hostBuilder
                .UseSetting(ConsoleHostDefaults.ApplicationKey, startupAssemblyName)
                .ConfigureServices(services =>
                {
                    if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                    {
                        services.AddSingleton(typeof(IStartup), startupType);
                    }
                    else
                    {
                        services.AddSingleton(typeof(IStartup), sp =>
                        {
                            var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                            return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, startupType, hostingEnvironment.EnvironmentName));
                        });
                    }
                });
        }

        /// <summary>
        /// Specify the startup type to be used by the console host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to configure.</param>
        /// <typeparam name ="TStartup">The type containing the startup methods for the application.</typeparam>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder UseStartup<TStartup>(this IConsoleHostBuilder hostBuilder) 
            where TStartup : class
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            return hostBuilder
                .UseStartup(typeof(TStartup));
        }

        /// <summary>
        /// Specify the environment to be used by the console host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to configure.</param>
        /// <param name="environment">The environment to host the application in.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder UseEnvironment(this IConsoleHostBuilder hostBuilder, string environment)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            if (environment == null)
                throw new ArgumentNullException(nameof(environment));

            return hostBuilder
                .UseSetting(ConsoleHostDefaults.EnvironmentKey, environment);
        }

        /// <summary>
        /// Specify the content root directory to be used by the console host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to configure.</param>
        /// <param name="contentRoot">Path to root directory of the application.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder UseContentRoot(this IConsoleHostBuilder hostBuilder, string contentRoot)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            if (contentRoot == null)
                throw new ArgumentNullException(nameof(contentRoot));

            return hostBuilder
                .UseSetting(ConsoleHostDefaults.ContentRootKey, contentRoot);
        }

        /// <summary>
        /// Specify the amount of time to wait for the console host to shutdown.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to configure.</param>
        /// <param name="timeout">The amount of time to wait for server shutdown.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder UseShutdownTimeout(this IConsoleHostBuilder hostBuilder, TimeSpan timeout)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            return hostBuilder
                .UseSetting(ConsoleHostDefaults.ShutdownTimeoutKey, ((int)timeout.TotalSeconds).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Start the console host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IConsoleHostBuilder"/> to start.</param>
        /// <param name="urls">The urls the hosted application will listen on.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHost Start(this IConsoleHostBuilder hostBuilder, params string[] urls)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            var host = hostBuilder.Build();
            host.Start();

            return host;
        }
    }
}