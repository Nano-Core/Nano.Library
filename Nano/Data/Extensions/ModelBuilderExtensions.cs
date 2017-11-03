using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Mappings;
using Nano.Models.Interfaces;

namespace Nano.Data.Extensions
{
    /// <summary>
    /// Model Builder Extensions.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Adds a mapping for the type <typeparamref name="TEntity"/> using the <typeparamref name="TMapping"/> implementation.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntity"/>.</typeparam>
        /// <typeparam name="TMapping">The <see cref="BaseEntityMapping{TEntity}"/>.</typeparam>
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