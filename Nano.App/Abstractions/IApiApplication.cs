using System;
using Microsoft.AspNetCore.Builder;

namespace Nano.App.Abstractions;

/// <summary>
/// Represents the contract for a Nano API application.
/// </summary>
public interface IApiApplication : IApplication<IApiApplication>
{
    /// <summary>
    /// Builds the application and finalizes configuration.
    /// Must be called before <see cref="IApplication.Run"/>.
    /// </summary>
    /// <param name="applicationBuilderAction">The <see cref="IApplicationBuilder"/>.</param>
    IApiApplication Build(Action<IApplicationBuilder>? applicationBuilderAction = null);
}