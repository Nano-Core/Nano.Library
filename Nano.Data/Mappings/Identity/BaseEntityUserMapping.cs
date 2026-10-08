using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models;
using System;

namespace Nano.Data.Mappings.Identity;

/// <inheritdoc />
public abstract class BaseEntityUserMapping<TEntity> : BaseEntityUserMapping<TEntity, Guid>
    where TEntity : BaseEntityUser;

/// <summary>
/// Configures the EF Core mapping for <see cref="BaseEntityUser{TIdentity}"/> entities.
/// Sets table name, query filters, indexes, properties, and relationships including cascading delete for the IdentityUser.
/// </summary>
/// <typeparam name="TEntity">The type of entity inheriting from <see cref="BaseEntityUser{TIdentity}"/>.</typeparam>
/// <typeparam name="TIdentity">The type of the entity's identity key.</typeparam>
public abstract class BaseEntityUserMapping<TEntity, TIdentity> : BaseEntityIdentityMapping<TEntity, TIdentity>
    where TEntity : BaseEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    internal override void ConfigureBase(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.ConfigureBase(builder);

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