using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.NLog.Extensions;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace Nano.Logging.NLog;

/// <summary>
/// A logging provider that configures NLog as the application's logging framework.
/// </summary>
public class NLogProvider : ILoggingProvider
{
    /// <summary>
    /// Configures NLog logging for the application using the provided <see cref="LoggingOptions"/>.
    /// <para>
    ///     This includes creating a console target with timestamped output, setting the default minimum log level,
    ///     applying namespace-specific log level overrides, and registering NLog with the <see cref="IServiceCollection"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register NLog services with.</param>
    /// <param name="options">The <see cref="LoggingOptions"/> controlling log levels and overrides.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="options"/> is <c>null</c>.</exception>
    public static void Configure(IServiceCollection services, LoggingOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        var configuration = new LoggingConfiguration();
        var target = new ConsoleTarget("console")
        {
            Layout = "${longdate} [${level:uppercase=true}] ${logger} - ${message}${exception:format=toString}"
        };

        configuration
            .AddTarget(target);

        configuration.LoggingRules
            .Add(new LoggingRule("*", options.LogLevel.GetLogLevel(), target));

        foreach (var @override in options.LogLevelOverrides)
        {
            configuration.LoggingRules
                .Add(new LoggingRule($"{@override.Namespace}.*", @override.LogLevel.GetLogLevel(), target));
        }

        services
            .AddLogging(x =>
            {
                x.AddNLog(configuration);
            });

        LogManager.Configuration = configuration;
    }
}