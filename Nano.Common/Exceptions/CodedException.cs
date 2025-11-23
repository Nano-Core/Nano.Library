using System;

namespace Nano.Common.Exceptions;

/// <summary>
/// Code Exception.
/// </summary>
public class CodedException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message.</param>
    public CodedException(string message)
        : base(message)
    {
    }
}