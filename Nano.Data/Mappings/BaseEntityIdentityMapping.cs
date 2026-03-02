using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Nano.Data.Abstractions.Models.Abstractions;
using System;

namespace Nano.Data.Mappings;

/// <summary>
/// Base mapping for entities implementing <see cref="IEntityIdentity{TIdentity}"/>.
/// Configures the primary key and value generator.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TIdentity">The type of the entity's identity key.</typeparam>
public abstract class BaseEntityIdentityMapping<TEntity, TIdentity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

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