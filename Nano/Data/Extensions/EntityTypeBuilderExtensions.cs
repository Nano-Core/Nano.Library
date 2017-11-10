using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Types;

namespace Nano.Data.Extensions
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
            where TEntity : class
        {
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
            where TEntity : class
        {
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
            where TEntity : class
        {
            builder
                .OwnsOne(expression)
                .Property(x => x.Meters)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Miles)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Meters);
        }

        /// <summary>
        /// Maps <see cref="Duration"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Duration>> expression)
            where TEntity : class
        {
            builder
                .OwnsOne(expression)
                .Property(x => x.Time)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Adjustment);

            builder
                .OwnsOne(expression)
                .Property(x => x.Total)
                .IsRequired();

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
            where TEntity : class
        {
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
        /// Maps <see cref="Percentage"/> as owned by the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="expression">The property expression.</param>
        public static void MapType<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Percentage>> expression)
            where TEntity : class
        {

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
            where TEntity : class
        {
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
            where TEntity : class
        {
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