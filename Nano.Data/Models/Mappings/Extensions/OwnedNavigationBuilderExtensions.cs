using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Types;

namespace Nano.Data.Models.Mappings.Extensions;

/// <summary>
/// Reference Collection Builder Extensions.
/// </summary>
public static class OwnedNavigationBuilderExtensions
{
    /// <summary>
    /// Maps <see cref="Address"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
    /// <param name="builder">The <see cref="OwnedNavigationBuilder{TEntity,TRelatedEntity}"/>.</param>
    /// <param name="expression">The <see cref="Expression"/>.</param>
    public static void MapType<TEntity, TRelatedEntity>(this OwnedNavigationBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Address>> expression)
        where TEntity : class
        where TRelatedEntity : class
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        builder
            .OwnsOne(expression)
            .Property(x => x.String)
            .HasMaxLength(256);

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.String);

        builder
            .OwnsOne(expression)
            .MapType(x => x.Location);
    }

    /// <summary>
    /// Maps <see cref="Distance"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
    /// <param name="builder">The <see cref="OwnedNavigationBuilder{TEntity,TRelatedEntity}"/>.</param>
    /// <param name="expression">The <see cref="Expression"/>.</param>
    public static void MapType<TEntity, TRelatedEntity>(this OwnedNavigationBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Distance>> expression)
        where TEntity : class
        where TRelatedEntity : class
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        builder
            .OwnsOne(expression)
            .Property(x => x.Meters)
            .HasDefaultValue(0.00)
            .IsRequired();

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.Meters);

        builder
            .OwnsOne(expression)
            .Property(x => x.Miles)
            .HasDefaultValue(0.00);

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.Miles);

        builder
            .OwnsOne(expression)
            .Property(x => x.Kilometers)
            .HasDefaultValue(0.00);

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.Kilometers);
    }

    /// <summary>
    /// Maps <see cref="Duration"/> for <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
    /// <param name="builder">The <see cref="OwnedNavigationBuilder{TEntity,TRelatedEntity}"/>.</param>
    /// <param name="expression">The <see cref="Expression"/>.</param>
    public static void MapType<TEntity, TRelatedEntity>(this OwnedNavigationBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Duration>> expression)
        where TEntity : class
        where TRelatedEntity : class
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        builder
            .OwnsOne(expression)
            .Property(x => x.Time)
            .HasDefaultValue(TimeSpan.Zero)
            .IsRequired();

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.Time);

        builder
            .OwnsOne(expression)
            .Property(x => x.Adjustment)
            .HasDefaultValue(TimeSpan.Zero)
            .IsRequired();

        builder
            .OwnsOne(expression)
            .Property(x => x.Total);

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.Total);
    }

    /// <summary>
    /// Maps <see cref="EmailAddress"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
    /// <param name="builder">The <see cref="OwnedNavigationBuilder{TEntity,TRelatedEntity}"/>.</param>
    /// <param name="expression">The <see cref="Expression"/>.</param>
    public static void MapType<TEntity, TRelatedEntity>(this OwnedNavigationBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, EmailAddress>> expression)
        where TEntity : class
        where TRelatedEntity : class
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (expression == null)
            throw new ArgumentNullException(nameof(expression));
        builder
            .OwnsOne(expression)
            .Property(x => x.Email)
            .HasMaxLength(256);

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.Email);

        builder
            .OwnsOne(expression)
            .Ignore(x => x.IsValid);
    }

    /// <summary>
    /// Maps <see cref="Location"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
    /// <param name="builder">The <see cref="OwnedNavigationBuilder{TEntity,TRelatedEntity}"/>.</param>
    /// <param name="expression">The <see cref="Expression"/>.</param>
    public static void MapType<TEntity, TRelatedEntity>(this OwnedNavigationBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Location>> expression)
        where TEntity : class
        where TRelatedEntity : class
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        builder
            .OwnsOne(expression)
            .Property(x => x.Latitude)
            .HasDefaultValue(0.00)
            .IsRequired();

        builder
            .OwnsOne(expression)
            .Property(x => x.Longitude)
            .HasDefaultValue(0.00)
            .IsRequired();

        builder
            .OwnsOne(expression)
            .HasIndex(x => new
            {
                x.Latitude,
                x.Longitude
            });
    }

    /// <summary>
    /// Maps <see cref="Percentage"/> for the <typeparamref name="TRelatedEntity"/>  owned by <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
    /// <param name="builder">The <see cref="OwnedNavigationBuilder{TEntity,TRelatedEntity}"/>.</param>
    /// <param name="expression">The <see cref="Expression"/>.</param>
    public static void MapType<TEntity, TRelatedEntity>(this OwnedNavigationBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Percentage>> expression)
        where TEntity : class
        where TRelatedEntity : class
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        builder
            .OwnsOne(expression)
            .Property(x => x.AsDecimal)
            .HasDefaultValue(0.00)
            .IsRequired();

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.AsDecimal);
    }

    /// <summary>
    /// Maps <see cref="PhoneNumber"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
    /// <param name="builder">The <see cref="OwnedNavigationBuilder{TEntity,TRelatedEntity}"/>.</param>
    /// <param name="expression">The <see cref="Expression"/>.</param>
    public static void MapType<TEntity, TRelatedEntity>(this OwnedNavigationBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, PhoneNumber>> expression)
        where TEntity : class
        where TRelatedEntity : class
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        builder
            .OwnsOne(expression)
            .Property(x => x.Number)
            .HasMaxLength(32);

        builder
            .OwnsOne(expression)
            .HasIndex(x => x.Number);
    }
}