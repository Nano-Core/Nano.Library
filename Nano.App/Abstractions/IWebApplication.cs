using System;
using Microsoft.AspNetCore.Builder;

namespace Nano.App.Abstractions;

/// <summary>
/// Represents the contract for a Nano Web application.
/// </summary>
public interface IWebApplication : IApplication<IWebApplication>
{
    /// <summary>
    /// Builds the application and finalizes configuration.
    /// Must be called before <see cref="IApplication.Run"/>.
    /// </summary>
    /// <typeparam name="TRoot">The root compoent type.</typeparam>
    /// <param name="applicationBuilderAction">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IWebApplication"/>.</returns>
    IWebApplication Build<TRoot>(Action<IApplicationBuilder>? applicationBuilderAction = null);
}