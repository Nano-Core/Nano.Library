using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models;
using Nano.Data.Mappings;

namespace Nano.Data.Identity.Mappings;

/// <inheritdoc />
public class DefaultEntityUserMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
    where TEntity : DefaultEntityUser
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<TEntity> builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

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