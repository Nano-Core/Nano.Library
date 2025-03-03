using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Nano.Models.Attributes;

/// <summary>
/// File Extension Validation Attribute.
/// </summary>
public class FileExtensionValidationAttribute : ValidationAttribute
{
    private readonly IEnumerable<string> allowedExtensions;

    /// <inheritdoc />
    public FileExtensionValidationAttribute(params string[] allowedExtensions)
    {
        this.allowedExtensions = allowedExtensions;
    }

    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (validationContext == null)
        {
            throw new ArgumentNullException(nameof(validationContext));
        }

        switch (value)
        {
            case IFormFile file:
            {
                var extension = Path.GetExtension(file.FileName);

                if (extension == null)
                {
                    throw new NullReferenceException(nameof(extension));
                }

                var isValid = this.allowedExtensions
                    .Select(x => x.ToLower())
                    .Contains(extension);

                if (isValid)
                {
                    return ValidationResult.Success;
                }

                break;
            }
            case IEnumerable<IFormFile> files:
            {
                var isValid = true;
                foreach (var extension in files.Select(file => Path.GetExtension(file.FileName)))
                {
                    if (extension == null)
                    {
                        throw new NullReferenceException(nameof(extension));
                    }

                    isValid = this.allowedExtensions
                        .Select(x => x.ToLower())
                        .Contains(extension);

                    if (!isValid)
                    {
                        break;
                    }
                }

                if (isValid)
                {
                    return ValidationResult.Success;
                }

                break;
            }
        }

        return new ValidationResult("The uploaded file has an invalid file extension.");
    }
}