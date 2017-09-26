using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Nano.Models.Interfaces;

namespace Nano.Models.Mappings
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

            base.Map(builder);

            builder
                .HasKey(y => y.Id);

            var propertyBuilder = builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            // TODO: Imrpove code quality.
            if (typeof(TIdentity) == typeof(long))
            {
                propertyBuilder.HasValueGenerator<ValueGenerator<long>>();
            }
            else if (typeof(TIdentity) == typeof(string))
            {
                propertyBuilder.HasValueGenerator<StringValueGenerator>();
            }
            else if (typeof(TIdentity) == typeof(Guid))
            {
                propertyBuilder.HasValueGenerator<GuidValueGenerator>();
            }
            else
            {
                throw new NotSupportedException("");
            }
        }
    }
}