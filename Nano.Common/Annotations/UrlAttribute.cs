using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Common.Annotations;

/// <summary>
/// Validates that a string property, field, or parameter contains a valid URL.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class UrlAttribute : ValidationAttribute
{
    private readonly UriKind uriKind;

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlAttribute"/> class.
    /// </summary>
    /// <param name="uriKind">Specifies the type of URI to validate. Defaults to <see cref="UriKind.Absolute"/>.</param>
    public UrlAttribute(UriKind uriKind = UriKind.Absolute)
    {
        this.uriKind = uriKind;
        ErrorMessage = "The value is not a valid URL.";
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not string url || string.IsNullOrWhiteSpace(url))
        {
            return new ValidationResult(ErrorMessage);
        }

        try
        {
            _ = new Uri(url, uriKind);

            return ValidationResult.Success;
        }
        catch (UriFormatException)
        {
            return new ValidationResult(this.ErrorMessage);
        }
    }
}