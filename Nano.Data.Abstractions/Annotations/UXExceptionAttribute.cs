using System;

namespace Nano.Data.Abstractions.Annotations;

/// <summary>
/// Marks a class with a user-facing exception message and associated properties.
/// This attribute can be applied multiple times to a single class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UxExceptionAttribute : Attribute
{
    /// <summary>
    /// Gets the message to be exposed to the user.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets the names of the properties associated with this exception.
    /// </summary>
    public string[] Properties { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UxExceptionAttribute"/> class.
    /// </summary>
    /// <param name="message">The user-facing message to expose.</param>
    /// <param name="properties">The properties associated with this exception.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> or <paramref name="properties"/> is <c>null</c>.</exception>
    public UxExceptionAttribute(string message, string[] properties)
    {
        this.Message = message ?? throw new ArgumentNullException(nameof(message));
        this.Properties = properties ?? throw new ArgumentNullException(nameof(properties));
    }
}