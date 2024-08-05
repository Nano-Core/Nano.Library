using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nano.Web.Hosting.ModelValidators;

/// <summary>
/// Guid Validator.
/// </summary>
public class GuidValidator : IModelValidator
{
    /// <inheritdoc/>
    public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
    {
        if (context == null) 
            throw new ArgumentNullException(nameof(context));

        if (context.Model is not Guid model || (Guid?)model == Guid.Empty)
        {
            var propertyName = context.ModelMetadata.Name;

            if (propertyName == null)
            {
                return new List<ModelValidationResult>();
            }

            var requiredAttribute = context.ModelMetadata.ContainerType?
                .GetProperty(propertyName)?
                .GetCustomAttribute<RequiredAttribute>();

            if (requiredAttribute == null)
            {
                return new List<ModelValidationResult>();
            }

            return new List<ModelValidationResult>
            {
                new(context.ModelMetadata.PropertyName, "The GUID must not be empty or invalid.")
            };
        }

        return new List<ModelValidationResult>();
    }
}