using Microsoft.Extensions.DependencyInjection;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.NLog.Extensions;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System;

namespace Nano.Logging.NLog;

/// <summary>
/// A logging provider that configures NLog as the application's logging framework.
/// </summary>
/// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.NLog</remarks>
public sealed class NLogProvider : ILoggingProvider
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
            Layout = "${date:format=dd-MM-yyyy HH\\:mm\\:ss.ffffff} [${level:uppercase=true:truncate=3}] ${message}${onexception:${newline}${exception:format=toString}}"
        };

        configuration
            .AddTarget(target);

        foreach (var @override in options.LogLevelOverrides)
        {
            var overrideNamePattern = $"{@override.Namespace}*";
            var overrideLogLevel = @override.LogLevel.GetLogLevel();
            var overrideLoggingRule = new LoggingRule(overrideNamePattern, overrideLogLevel, target)
            {
                FinalMinLevel = overrideLogLevel
            };

            configuration.LoggingRules
                .Add(overrideLoggingRule);
        }

        const string NAME_PATTERN = "*";
        var logLevel = options.LogLevel.GetLogLevel();
        var loggingRule = new LoggingRule(NAME_PATTERN, logLevel, target);

        configuration.LoggingRules
            .Add(loggingRule);

        services
            .AddLogging(x =>
            {
                x.AddNLog(configuration);
            });

        LogManager.Configuration = configuration;
    }
}