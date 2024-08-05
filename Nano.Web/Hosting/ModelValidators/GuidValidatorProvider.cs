using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nano.Web.Hosting.ModelValidators;

/// <summary>
/// Guid Validator Provider.
/// </summary>
public class GuidValidatorProvider : IModelValidatorProvider
{
    /// <inheritdoc/>
    public void CreateValidators(ModelValidatorProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.ModelMetadata.ModelType == typeof(Guid) || context.ModelMetadata.ModelType == typeof(Guid?))
        {
            context.Results
                .Add(new ValidatorItem
                {
                    Validator = new GuidValidator(),
                    IsReusable = true
                });
        }
    }
}