using System;
using System.Linq.Expressions;
using EntityFrameworkCore.Triggers;
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
        /// Adds an inserted event trigger to the model.
        /// The passed <paramref name="action"/> will be invoked, after the entity of type <typeparamref name="TEntity"/> is inserted.
        /// </summary>
        /// <typeparam name="TEntity">the type of entity.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="action">The <see cref="Action"/> invoked.</param>
        public static void OnInserted<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IInsertedEntry<TEntity>> action)
            where TEntity : class, IEntityCreatable
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Triggers<TEntity>.Inserted += action;
        }

        /// <summary>
        /// Adds an inserting event trigger to the model.
        /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is inserted.
        /// </summary>
        /// <typeparam name="TEntity">the type of entity.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="action">The <see cref="Action"/> invoked.</param>
        public static void OnInserting<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IInsertingEntry<TEntity>> action)
            where TEntity : class, IEntityCreatable
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Triggers<TEntity>.Inserting += action;
        }
        
        /// <summary>
        /// Adds an updated event trigger to the model.
        /// The passed <paramref name="action"/> will be invoked, after the entity of type <typeparamref name="TEntity"/> is updated.
        /// </summary>
        /// <typeparam name="TEntity">the type of entity.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="action">The <see cref="Action"/> invoked.</param>
        public static void OnUpdated<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IUpdatedEntry<TEntity>> action)
            where TEntity : class, IEntityUpdatable
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Triggers<TEntity>.Updated += action;
        }

        /// <summary>
        /// Adds an updating event trigger to the model.
        /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is updated.
        /// </summary>
        /// <typeparam name="TEntity">the type of entity.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="action">The <see cref="Action"/> invoked.</param>
        public static void OnUpdating<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IUpdatingEntry<TEntity>> action)
            where TEntity : class, IEntityUpdatable
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Triggers<TEntity>.Updating += action;
        }

        /// <summary>
        /// Adds an deleted event trigger to the model.
        /// The passed <paramref name="action"/> will be invoked, after the entity of type <typeparamref name="TEntity"/> is deleted.
        /// </summary>
        /// <typeparam name="TEntity">the type of entity.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="action">The <see cref="Action"/> invoked.</param>
        public static void OnDeleted<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeletedEntry<TEntity>> action)
            where TEntity : class, IEntityDeletableSoft
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Triggers<TEntity>.Deleted += action;
        }

        /// <summary>
        /// Adds an deleted event trigger to the model.
        /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is deleted.
        /// </summary>
        /// <typeparam name="TEntity">the type of entity.</typeparam>
        /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
        /// <param name="action">The <see cref="Action"/> invoked.</param>
        public static void OnDeleting<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeletingEntry<TEntity>> action)
            where TEntity : class, IEntityDeletableSoft
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Triggers<TEntity>.Deleting += action;
        }
        
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