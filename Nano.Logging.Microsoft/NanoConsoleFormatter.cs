using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Nano.Logging.Microsoft;

/// <summary>
/// A custom console formatter that outputs log entries in a concise, Serilog-inspired format for consistent cross-provider logging.
/// </summary>
public sealed class NanoConsoleFormatter() : ConsoleFormatter(NanoConsoleFormatter.FORMATTER_NAME)
{
    internal const string FORMATTER_NAME = "Nano";

    /// <summary>
    /// Writes a formatted log entry to the console output.
    /// </summary>
    /// <typeparam name="TState">The type of the log state object.</typeparam>
    /// <param name="logEntry">The log entry containing log level, message, exception, and state.</param>
    /// <param name="scopeProvider">The scope provider for the log entry. Scopes are intentionally ignored.</param>
    /// <param name="textWriter">The writer used to output the formatted log message.</param>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var timestamp = DateTime.Now
            .ToString("dd-MM-yyyy HH:mm:ss.ffffff");

        var level = logEntry.LogLevel
            .ToString()
            .ToUpperInvariant()[..3];

        textWriter
            .Write($"{timestamp} [{level}] ");

        textWriter
            .Write(logEntry.Formatter
                .Invoke(logEntry.State, logEntry.Exception));

        if (logEntry.Exception != null)
        {
            textWriter
                .WriteLine();
            
            textWriter
                .Write(logEntry.Exception);
        }

        textWriter
            .WriteLine();
    }
}