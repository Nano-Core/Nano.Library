using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Nano.Common.Config;

/// <summary>
/// Implementation of <see cref="IValidateOptions{TOptions}"/> that uses DataAnnotation's <see cref="Validator"/> for validation.
/// </summary>
/// <typeparam name="TOptions">The instance being validated.</typeparam>
public class DataAnnotationsValidateRecursiveOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name"></param>
    public DataAnnotationsValidateRecursiveOptions(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        this.Name = name;
    }

    /// <summary>
    /// The options name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Validates a specific named options instance (or all when name is null).
    /// </summary>
    /// <param name="name">The name of the options instance being validated.</param>
    /// <param name="options">The options instance.</param>
    /// <returns>The <see cref="ValidateOptionsResult"/> result.</returns>
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (name != this.Name)
        {
            return ValidateOptionsResult.Skip;
        }

        var validationResults = new List<ValidationResult>();

        var validateObjectRecursive = DataAnnotationsValidator
            .TryValidateObjectRecursive(options, validationResults);

        return validateObjectRecursive
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(string.Join(Environment.NewLine, validationResults.Select(r => "DataAnnotation validation failed for members " + string.Join((string?)", ", (IEnumerable<string?>)r.MemberNames) + " with the error '" + r.ErrorMessage + "'.")));
    }
}