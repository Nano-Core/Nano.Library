using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models;
using System;

namespace Nano.Data.Mappings;

/// <inheritdoc />
public abstract class BaseEntityMapping<TEntity> : BaseEntityMapping<TEntity, Guid>
    where TEntity : BaseEntity
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);
    }
}

/// <summary>
/// Base mapping class for EF Core entities.
/// Provides an abstract method to configure the entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to map.</typeparam>
/// <typeparam name="TIdentity">The type of the entity's identity key.</typeparam>
public abstract class BaseEntityMapping<TEntity, TIdentity> : BaseEntityIdentityMapping<TEntity, TIdentity>
    where TEntity : BaseEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Configures the EF Core model for <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity.</param>
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .HasQueryFilter(x => x.IsDeleted == 0L);

        builder
            .Property(x => x.CreatedAt)
            .ValueGeneratedOnAdd()
            .IsRequired()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder
            .HasIndex(x => x.CreatedAt);

        builder
            .Property(y => y.IsDeleted)
            .HasDefaultValue(0L)
            .IsRequired();

        builder
            .HasIndex(x => x.IsDeleted);
    }
}