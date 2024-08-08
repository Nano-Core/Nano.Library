using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Attributes;

/// <summary>
/// Guid Not Empty Attribute.
/// </summary>
public class GuidNotEmptyAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is Guid guid && guid == Guid.Empty)
        {
            return new ValidationResult($"{validationContext.DisplayName} must not be empty.");
        }

        return ValidationResult.Success;
    }
}