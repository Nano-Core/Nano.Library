using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Types;

namespace Nano.Data.Models.Mappings.Extensions
{
    // TODO: Figure out how to handle that compunds should be able to be optional. Maybe default all properties or add a "isRequired" parameter or something.

    /// <summary>
    /// Reference Collection Builder Extensions.
    /// </summary>
    public static class ReferenceCollectionBuilderExtensions
    {
        /// <summary>
        /// Maps <see cref="Angle"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Angle>> expression)
            where TEntity : class
            where TRelatedEntity : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Radians)
                .HasField("radians")
                .HasDefaultValue(0.00M)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Degrees)
                .HasField("degrees")
                .HasDefaultValue(0.00M)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Ignore(x => x.Radians2Pi);

            builder
                .OwnsOne(expression)
                .Ignore(x => x.Degrees360);
        }

        /// <summary>
        /// Maps <see cref="Range"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Range>> expression)
            where TEntity : class
            where TRelatedEntity : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Lower)
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Upper);
        }

        /// <summary>
        /// Maps <see cref="Photo"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Photo>> expression)
            where TEntity : class
            where TRelatedEntity : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Url)
                .HasMaxLength(1024)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Reference)
                .HasMaxLength(1024);

            builder
                .OwnsOne(expression)
                .Property(x => x.Alt)
                .HasMaxLength(256);
        }

        /// <summary>
        /// Maps <see cref="Address"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Address>> expression)
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
                .HasMaxLength(256)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.String);

            builder
                .OwnsOne(expression)
                .MapType(x => x.Location);
        }

        /// <summary>
        /// Maps <see cref="Contact"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Contact>> expression)
            where TEntity : class
            where TRelatedEntity : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Name);

            builder
                .OwnsOne(expression)
                .MapType(x => x.PhoneNumber);

            builder
                .OwnsOne(expression)
                .MapType(x => x.EmailAddress);
        }

        /// <summary>
        /// Maps <see cref="Period"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Period>> expression)
            where TEntity : class
            where TRelatedEntity : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.BeginAt)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.FinishAt)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => new
                {
                    x.BeginAt,
                    x.FinishAt
                });
        }

        /// <summary>
        /// Maps <see cref="Distance"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Distance>> expression)
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
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Meters);

            builder
                .OwnsOne(expression)
                .Property(x => x.Miles);

            builder
                .OwnsOne(expression)
                .Property(x => x.Kilometers);
        }

        /// <summary>
        /// Maps <see cref="Duration"/> for <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Duration>> expression)
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
        /// Maps <see cref="Location"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Location>> expression)
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
        /// Maps <see cref="PhoneNumber"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, PhoneNumber>> expression)
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
                .HasMaxLength(32)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Number);
        }

        /// <summary>
        /// Maps <see cref="Percentage"/> for the <typeparamref name="TRelatedEntity"/>  owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, Percentage>> expression)
            where TEntity : class
            where TRelatedEntity : class
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
        /// Maps <see cref="EmailAddress"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, EmailAddress>> expression)
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
                .HasMaxLength(256)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Ignore(x => x.IsValid);

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Email);
        }

        /// <summary>
        /// Maps <see cref="AuthenticationCredential"/> for the <typeparamref name="TRelatedEntity"/> owned by <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
        /// <param name="builder">The <see cref="ReferenceOwnershipBuilder{TEntity,TRelatedEntity}"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        public static void MapType<TEntity, TRelatedEntity>(this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> builder, Expression<Func<TRelatedEntity, AuthenticationCredential>> expression)
            where TEntity : class
            where TRelatedEntity : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            builder
                .OwnsOne(expression)
                .Property(x => x.Username)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .Property(x => x.Password)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .OwnsOne(expression)
                .HasIndex(x => x.Username)
                .IsUnique();
        }
    }
}