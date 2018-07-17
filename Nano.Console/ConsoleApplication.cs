using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App;
using Nano.App.Extensions;
using Nano.App.Interfaces;
using Nano.Config;
using Nano.Config.Extensions;
using Nano.Console.Hosting;
using Nano.Console.Hosting.Extensions;
using Nano.Console.Hosting.Interfaces;
using Nano.Data.Extensions;
using Nano.Eventing.Extensions;
using Nano.Logging.Extensions;
using Nano.Security.Extensions;

namespace Nano.Console
{
    // TODO: Console Application, complete Test and documentation.

    /// <inheritdoc />
    public class ConsoleApplication : DefaultApplication
    {
        /// <inheritdoc />
        public ConsoleApplication(IConfiguration configuration)
            : base(configuration)
        {

        }

        /// <summary>
        /// Creates a <see cref="IConsoleHostBuilder"/>.
        /// The application startup implementation is defined by the generic type parameter <typeparamref name="TApplication"/>.
        /// </summary>
        /// <typeparam name="TApplication">The type containing method for application start-up.</typeparam>
        /// <param name="args">The command-line args, if any.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        public static IConsoleHostBuilder ConfigureApp<TApplication>(params string[] args)
            where TApplication : class, IApplication, new()
        {
            var root = Directory.GetCurrentDirectory();
            var config = ConfigManager.BuildConfiguration(args);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var services = new ServiceCollection();
            var application = new ConsoleApplication(config);
            application.ConfigureServices(services);

            return new ConsoleHostBuilder()
                .UseContentRoot(root)
                .UseEnvironment(environment)
                .UseConfiguration(config)
                .ConfigureServices(x =>
                {
                    x.AddSingleton<IApplication, TApplication>();

                    x.AddApp(config);
                    x.AddData(config);
                    x.AddConfig(config);
                    x.AddLogging(config);
                    x.AddSecurity(config);
                    x.AddEventing(config);

                    x.AddConsole(config);
                })
                .UseStartup<TApplication>()
                .CaptureStartupErrors(true);
        }
    }
}