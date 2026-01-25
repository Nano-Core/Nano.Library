using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Entities;

namespace Nano.Data.Mappings.Identity;

/// <summary>
/// Configures the EF Core mapping for <see cref="DefaultEntityUser"/> entities.
/// Sets table name, query filters, indexes, properties, and relationships including cascading delete for the IdentityUser.
/// </summary>
/// <typeparam name="TEntity">The type of entity inheriting from <see cref="DefaultEntityUser"/>.</typeparam>
public class DefaultEntityUserMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
    where TEntity : DefaultEntityUser
{
    /// <summary>
    /// Configures the entity using the <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public override void Map(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Map(builder);

        builder
            .HasQueryFilter(x => x.IsDeleted == 0L && x.IdentityUser.IsActive);

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

        builder
            .HasOne(x => x.IdentityUser)
            .WithOne()
            .HasForeignKey<TEntity>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}