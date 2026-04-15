using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nano.Common.Config.Extensions;

/// <summary>
/// Extension methods for adding configuration related options services to the DI container via <see cref="OptionsBuilder{TOptions}"/>.
/// </summary>
public static class OptionsBuilderExtensions
{
    /// <summary>
    /// Register this options instance for validation of its DataAnnotations.
    /// </summary>
    /// <typeparam name="TOptions">The options type to be configured.</typeparam>
    /// <param name="optionsBuilder">The options builder to add the services to.</param>
    /// <returns>The <see cref="OptionsBuilder{TOptions}"/> so that additional calls can be chained.</returns>
    public static OptionsBuilder<TOptions> ValidateDataAnnotationsRecursively<TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        optionsBuilder.Services
            .AddSingleton<IValidateOptions<TOptions>>(new DataAnnotationsValidateRecursiveOptions<TOptions>(optionsBuilder.Name));

        return optionsBuilder;
    }
}