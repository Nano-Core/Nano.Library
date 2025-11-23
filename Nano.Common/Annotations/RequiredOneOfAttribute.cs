using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Nano.Models.Attributes;

/// <summary>
/// Required One Of Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RequiredOneOfAttribute : ValidationAttribute
{
    private readonly string[] otherProperties;

    /// <inheritdoc />
    public RequiredOneOfAttribute(params string[] otherProperties)
    {
        this.otherProperties = otherProperties ?? throw new ArgumentNullException(nameof(otherProperties));
    }

    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            return ValidationResult.Success;
        }

        foreach (var propertyName in this.otherProperties)
        {
            var property = validationContext.ObjectType
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new NullReferenceException(nameof(property));
            }

            var propertyValue = property
                .GetValue(validationContext.ObjectInstance);

            if (propertyValue != null)
            {
                return ValidationResult.Success;
            }
        }

        var allProperties = this.otherProperties.Append(validationContext.MemberName);

        return new ValidationResult(ErrorMessage ?? $"At least one of the properties ({string.Join(", ", allProperties)}) must be set.");
    }
}