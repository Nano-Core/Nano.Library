using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Common.Annotations;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class UrlAttribute : ValidationAttribute
{
    private readonly UriKind uriKind;

    /// <inheritdoc />
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