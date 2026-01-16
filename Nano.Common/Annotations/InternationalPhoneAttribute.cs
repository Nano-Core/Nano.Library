using System;
using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace Nano.Common.Annotations;

/// <summary>
/// International Phone Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class InternationalPhoneAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ArgumentNullException.ThrowIfNull(validationContext);

        var phone = value as string;

        if (string.IsNullOrWhiteSpace(phone))
        {
            return ValidationResult.Success;
        }

        var phoneNumberUtil = PhoneNumberUtil.GetInstance();

        try
        {
            var parsedPhone = phoneNumberUtil
                .Parse(phone, null);

            return phoneNumberUtil.IsValidNumber(parsedPhone)
                ? ValidationResult.Success
                : new ValidationResult("Invalid international phone number.");
        }
        catch (NumberParseException)
        {
            return new ValidationResult("Invalid international phone number.");
        }
    }
}