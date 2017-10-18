using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Nano.App.Models.Interfaces;

namespace Nano.App.Models.Mappings
{
    /// <inheritdoc />
    public abstract class BaseEntityIdentityMapping<TEntity, TIdentity> : BaseEntityMapping<TEntity>
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder
                .HasKey(y => y.Id);

            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasValueGenerator(
                    typeof(TIdentity) == typeof(Guid)
                        ? typeof(GuidValueGenerator)
                        : typeof(ValueGenerator<TIdentity>));
        }
    }
}