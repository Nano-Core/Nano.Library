using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nano.Console.Hosting.Interfaces
{
    /// <summary>
    /// A builder for <see cref="IConsoleHost"/>.
    /// </summary>
    public interface IConsoleHostBuilder
    {
        /// <summary>
        /// Builds an <see cref="IConsoleHost"/> which hosts a console application.
        /// </summary>
        IConsoleHost Build();

        /// <summary>
        /// Adds a delegate for configuring the <see cref="IConfigurationBuilder"/> that will construct an <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used to construct an <see cref="IConfiguration" />.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        /// <remarks>
        /// The <see cref="IConfiguration"/> and <see cref="ILoggerFactory"/> on the <see cref="ConsoleHostBuilderContext"/> are uninitialized at this stage.
        /// The <see cref="IConfigurationBuilder"/> is pre-populated with the settings of the <see cref="IConsoleHostBuilder"/>.
        /// </remarks>
        IConsoleHostBuilder ConfigureAppConfiguration(Action<ConsoleHostBuilderContext, IConfigurationBuilder> configureDelegate);

        /// <summary>
        /// Adds a delegate for configuring additional services for the host or console application. This may be called
        /// multiple times.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        IConsoleHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        /// <summary>
        /// Adds a delegate for configuring additional services for the host or console application. This may be called
        /// multiple times.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        IConsoleHostBuilder ConfigureServices(Action<ConsoleHostBuilderContext, IServiceCollection> configureServices);

        /// <summary>
        /// Get the setting value from the configuration.
        /// </summary>
        /// <param name="key">The key of the setting to look up.</param>
        /// <returns>The value the setting currently contains.</returns>
        string GetSetting(string key);

        /// <summary>
        /// Add or replace a setting in the configuration.
        /// </summary>
        /// <param name="key">The key of the setting to add or replace.</param>
        /// <param name="value">The value of the setting to add or replace.</param>
        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
        IConsoleHostBuilder UseSetting(string key, string value);
    }
}