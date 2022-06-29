using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nano.Data.Models.Mappings.Extensions;

/// <summary>
/// Property Builder Extensions.
/// </summary>
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Adds precision and scale to a decimal column in the database.
    /// </summary>
    /// <param name="builder">The <see cref="PropertyBuilder{TProperty}"/>.</param>
    /// <param name="precision">The precision.</param>
    /// <param name="scale">The scale.</param>
    /// <returns>The <see cref="PropertyBuilder{TProperty}"/>.</returns>
    public static PropertyBuilder<decimal> HasPrecision(this PropertyBuilder<decimal> builder, int precision, int scale)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder
            .HasColumnType($"decimal({precision},{scale})");
    }

    /// <summary>
    /// Adds precision and scale to a decimal column in the database.
    /// </summary>
    /// <param name="builder">The <see cref="PropertyBuilder{TProperty}"/>.</param>
    /// <param name="precision">The precision.</param>
    /// <param name="scale">The scale.</param>
    /// <returns>The <see cref="PropertyBuilder{TProperty}"/>.</returns>
    public static PropertyBuilder<decimal?> HasPrecision(this PropertyBuilder<decimal?> builder, int precision, int scale)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder
            .HasColumnType($"decimal({precision},{scale})");
    }
}