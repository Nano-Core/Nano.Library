using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Base class for external providers.
/// </summary>
public abstract class BaseExternalProvider
{
    /// <summary>
    /// The unique name of the external provider.
    /// </summary>
    [Required]
    public virtual string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseExternalProvider"/> class with the specified provider name.
    /// </summary>
    /// <param name="name">The unique name of the external authentication provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
    protected BaseExternalProvider(string name)
    {
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}