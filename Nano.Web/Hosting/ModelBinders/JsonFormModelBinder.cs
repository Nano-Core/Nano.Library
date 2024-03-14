using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nano.Web.Hosting.ModelBinders;

/// <summary>
/// Json Form binder.
/// </summary>
public class JsonFormModelBinder : IModelBinder
{
    /// <summary>
    /// Deserializes the json value and binds it to its type.
    /// </summary>
    /// <param name="bindingContext">The <see cref="ModelBindingContext"/>.</param>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var valueProviderResult = bindingContext.ValueProvider
            .GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return;
        }

        bindingContext.ModelState
            .SetModelValue(bindingContext.ModelName, valueProviderResult);

        var serialized = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(serialized))
        {
            bindingContext.Result = ModelBindingResult
                .Success(null);

            return;
        }

        try
        {
            object deserialized;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
            {
                deserialized = await JsonSerializer
                    .DeserializeAsync(stream, bindingContext.ModelType);
            }

            if (deserialized == null)
            {
                bindingContext.ModelState
                    .AddModelError(bindingContext.ModelName, "Failed to deserialize JSON.");

                return;
            }

            var validationResultProps = TypeDescriptor
                .GetProperties(deserialized)
                .Cast<PropertyDescriptor>()
                .SelectMany(property => property.Attributes
                    .OfType<ValidationAttribute>()
                    .Where(attribute => !attribute.IsValid(property.GetValue(deserialized))))
                .Select(attribute => new
                {
                    Member = attribute.FormatErrorMessage(string.Empty),
                    ErrorMessage = attribute.FormatErrorMessage(string.Empty)
                });

            var validationResultFields = TypeDescriptor
                .GetReflectionType(deserialized)
                .GetFields()
                .SelectMany(field => field
                    .GetCustomAttributes<ValidationAttribute>()
                    .Where(attribute => !attribute.IsValid(field.GetValue(deserialized))))
                .Select(attribute => new
                {
                    Member = attribute.FormatErrorMessage(string.Empty),
                    ErrorMessage = attribute.FormatErrorMessage(string.Empty)
                });

            var errors = validationResultFields
                .Concat(validationResultProps);

            foreach (var validationResultItem in errors)
            {
                bindingContext.ModelState
                    .AddModelError(validationResultItem.Member, validationResultItem.ErrorMessage);
            }

            bindingContext.Result = ModelBindingResult
                .Success(deserialized);
        }
        catch (JsonException e)
        {
            bindingContext.ModelState
                .AddModelError(bindingContext.ModelName, e.Message);
        }
    }
}