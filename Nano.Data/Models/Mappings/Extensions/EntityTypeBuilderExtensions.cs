using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;
using Nano.Models.Types;

namespace Nano.Data.Models.Mappings.Extensions
{
    /// <summary>
    /// Entity Type Builder Extensions.
    /// </summary>
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// Maps <see cref="Address"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Address>> expression)
            where TEntity : class, IEntity
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
        /// Maps <see cref="Distance"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Distance>> expression)
            where TEntity : class, IEntity
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
                .HasDefaultValue(0.00)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Miles);

            builder
                .OwnsOne(expression)
                .Property(x => x.Kilometers)
                .HasDefaultValue(0.00)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Kilometers);
        }

        /// <summary>
        /// Maps <see cref="Duration"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Duration>> expression)
            where TEntity : class, IEntity
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
        /// Maps <see cref="EmailAddress"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, EmailAddress>> expression)
            where TEntity : class, IEntity
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
        /// Maps <see cref="Location"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Location>> expression)
            where TEntity : class, IEntity
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
        /// Maps <see cref="Percentage"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Percentage>> expression)
            where TEntity : class, IEntity
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.AsDecimal)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.AsDecimal);

            builder
                .OwnsOne(expression)
                .Ignore(x => x.AsInteger);
        }

        /// <summary>
        /// Maps <see cref="PhoneNumber"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, PhoneNumber>> expression)
            where TEntity : class, IEntity
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Number)
                .HasMaxLength(20);

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Number);
        }
    }
}