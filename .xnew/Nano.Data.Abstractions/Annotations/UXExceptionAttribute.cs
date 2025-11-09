using System;

namespace Nano.Models.Attributes;

/// <summary>
/// Ux Exception Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UxExceptionAttribute : Attribute
{
    /// <summary>
    /// Message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Properties.
    /// </summary>
    public string[] Properties { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message to expose.</param>
    /// <param name="properties">The properties.</param>
    public UxExceptionAttribute(string message, string[] properties)
    {
        this.Message = message ?? throw new ArgumentNullException(nameof(message));
        this.Properties = properties ?? throw new ArgumentNullException(nameof(properties));
    }
}