using System;

namespace Nano.App.Exceptions;

/// <summary>
/// Represents an exception that is thrown for a bad request errors.
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    /// Indicates whether the <see cref="Exception.Message"/> is a unique code.
    /// </summary>
    public bool IsCoded { get; set; }

    /// <summary>
    /// Indicates whether the <see cref="Exception.Message"/> is a translated message.
    /// </summary>
    public bool IsTranslated { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    public BadRequestException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="isCoded">Whether the <paramref name="message"/> is a unique code.</param>
    /// <param name="isTranslated">Whether the <paramref name="message"/> is a translated message.</param>
    public BadRequestException(string message, bool isCoded = false, bool isTranslated = false)
        : base(message)
    {
        this.IsCoded = isCoded;
        this.IsTranslated = isTranslated;
    }
}