using System;

namespace Nano.Data.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that occurs when a database unique constraint is violated, typically caused by an attempt to create or update an entity
/// that conflicts with an existing unique index or constraint.
/// </summary>
public sealed class UniqueConstraintViolationException : Exception
{
    private const string MESSAGE = "Duplicate entity conflict. A unique constraint was violated by the operation.";

    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraintViolationException"/> class.
    /// </summary>
    /// <param name="inner">The underlying exception that caused the unique constraint violation, if available.</param>
    public UniqueConstraintViolationException(Exception? inner = null)
        : base(MESSAGE, inner)
    {
    }
}