using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Mappings;
using Nano.Models.Interfaces;

namespace Nano.App.Data.Extensions
{
    /// <summary>
    /// Model Builder Extensions.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Adds a mapping for the type <typeparamref name="TEntity"/> using the <typeparamref name="TMapping"/> implementation.
        /// </summary>
        /// <typeparam name="TEntity">The entity to add mapping for.</typeparam>
        /// <typeparam name="TMapping">The entity mapping implementation to use for configuring the entity context.</typeparam>
        /// <param name="builder">The <see cref="ModelBuilder"/>.</param>
        /// <returns>The <see cref="ModelBuilder"/>.</returns>
        public static ModelBuilder AddMapping<TEntity, TMapping>(this ModelBuilder builder)
            where TEntity : class, IEntity
            where TMapping : BaseEntityMapping<TEntity>, new()
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var entityMap = new TMapping();

            entityMap
                .Map(builder.Entity<TEntity>());

            return builder;
        }
    }
}