using System;

namespace Nano.Data.Abstractions;

/// <summary>
/// Provides a mechanism for translating low-level database or ORM-specific exceptions into application-level exceptions that are
/// independent of any specific data provider.
/// </summary>
public interface IDatabaseExceptionTranslator
{
    /// <summary>
    /// Translates the specified <paramref name="exception"/> into a more meaningful or domain-specific exception when possible.
    /// </summary>
    /// <param name="exception">The exception to evaluate and potentially translate. This may be a database,ORM, or any other infrastructure-level exception.</param>
    /// <returns>A translated exception if a known pattern is recognized; otherwise, the original<paramref name="exception"/> is returned unchanged.</returns>
    Exception Translate(Exception exception);
}