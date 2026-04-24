using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Nano.Common.Annotations;

/// <summary>
/// Validates that at least one of the specified members, including the decorated member,
/// has a non-null value. Works with properties, fields, and parameters.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class RequiredOneOfAttribute : ValidationAttribute
{
    private readonly string[] otherMembers;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredOneOfAttribute"/> class.
    /// </summary>
    /// <param name="otherMembers">An array of member names (properties or fields) that should be checked alongside the decorated member.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="otherMembers"/> is null.</exception>
    public RequiredOneOfAttribute(params string[] otherMembers)
    {
        this.otherMembers = otherMembers ?? throw new ArgumentNullException(nameof(otherMembers));
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null)
        {
            return ValidationResult.Success;
        }

        var type = validationContext.ObjectType;
        var instance = validationContext.ObjectInstance;

        foreach (var memberName in this.otherMembers)
        {
            var property = type
                .GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                var propertyValue = property
                    .GetValue(instance);

                if (propertyValue != null)
                {
                    return ValidationResult.Success;
                }

                continue;
            }

            var field = type
                .GetField(memberName, BindingFlags.Public | BindingFlags.Instance);

            if (field != null)
            {
                var fieldValue = field
                    .GetValue(instance);

                if (fieldValue != null)
                {
                    return ValidationResult.Success;
                }

                continue;
            }

            throw new NullReferenceException($"Member '{memberName}' not found on type '{type.FullName}'.");
        }

        var allMembers = this.otherMembers
            .Append(validationContext.MemberName);
        return new ValidationResult(this.ErrorMessage ?? $"At least one of the members ({string.Join(", ", allMembers)}) must be set.");
    }
}
