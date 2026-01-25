using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Entities;

namespace Nano.Data.Mappings;

/// <summary>
/// Default mapping for <see cref="DefaultEntity{Guid}"/>.
/// Configures soft delete filter, CreatedAt, IsDeleted, and indexes.
/// </summary>
/// <typeparam name="TEntity">The entity type inheriting <see cref="DefaultEntity{Guid}"/>.</typeparam>
public class DefaultEntityMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
    where TEntity : DefaultEntity<Guid>
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Map(builder);

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