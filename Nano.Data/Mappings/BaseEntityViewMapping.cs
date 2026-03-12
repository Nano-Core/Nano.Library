using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Abstractions;
using System;

namespace Nano.Data.Mappings;

/// <inheritdoc />
public abstract class BaseEntityViewMapping<TEntity> : BaseEntityViewMapping<TEntity, Guid>
    where TEntity : class, IEntityReadOnly<Guid> // BUG:
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToView(typeof(TEntity).Name)
            .HasNoKey();
    }
}

/// <summary>
/// Base mapping class for read-only entities mapped to database views.
/// Configures the entity to have no key and maps it to a view.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to map.</typeparam>
/// <typeparam name="TIdentity">The type of the entity's identity key.</typeparam>
public abstract class BaseEntityViewMapping<TEntity, TIdentity> : BaseMapping<TEntity, TIdentity>
    where TEntity : class, IEntityReadOnly<TIdentity>
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