using System;
using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace Nano.Models.Attributes;

/// <summary>
/// International Phone Attribute.
/// </summary>
public class InternationalPhoneAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (validationContext == null) 
            throw new ArgumentNullException(nameof(validationContext));
        
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

            if (phoneNumberUtil.IsValidNumber(parsedPhone))
            {
                return ValidationResult.Success;
            }
        }
        catch (NumberParseException)
        {
        }

        return new ValidationResult("Invalid international phone number.");
    }
}