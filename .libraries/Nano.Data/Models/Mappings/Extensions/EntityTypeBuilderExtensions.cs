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
        /// Maps <see cref="Angle"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Angle>> expression)
            where TEntity : class, IEntity
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Radians)
                .HasField("radians")
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Degrees)
                .HasField("degrees")
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Ignore(x => x.Radians2Pi);

            builder
                .OwnsOne(expression)
                .Ignore(x => x.Degrees360);
        }

        /// <summary>
        /// Maps <see cref="Period"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Period>> expression)
            where TEntity : class, IEntity
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Start)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Finish)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => new
                {
                    x.Start,
                    x.Finish
                });
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
                .HasDefaultValue(0.00);

            builder
                .OwnsOne(expression)
                .Property(x => x.Kilometers)
                .HasDefaultValue(0.00);
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
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Time);

            builder
                .OwnsOne(expression)
                .Property(x => x.Adjustment)
                .HasDefaultValue(TimeSpan.Zero);

            builder
                .OwnsOne(expression)
                .Property(x => x.Total);

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Total);
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
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Longitude)
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
                .HasMaxLength(20)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Number);
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
                .Ignore(x => x.AsInteger);

            builder
                .OwnsOne(expression)
                .Property(x => x.AsDecimal)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.AsDecimal);
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
                .HasMaxLength(254)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Ignore(x => x.IsValid);

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Email);
        }

        /// <summary>
        /// Maps <see cref="AuthenticationCredential"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, AuthenticationCredential>> expression)
            where TEntity : class, IEntity
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Username)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Password)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Username)
                .IsUnique();

            builder
                .OwnsOne(expression)
                .Property(x => x.Token)
                .HasMaxLength(255);

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Token);
        }
    }
}