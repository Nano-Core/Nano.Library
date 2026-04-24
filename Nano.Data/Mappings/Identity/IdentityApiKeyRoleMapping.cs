using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Consts;

namespace Nano.Data.Mappings.Identity;

/// <summary>
/// Configures the EF Core mapping for <see cref="IdentityApiKeyRole{TIdentity}"/> entities.
/// Sets table name, query filters, indexes, and relationships.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityApiKeyRoleMapping<TIdentity> : BaseEntityIdentityMapping<IdentityApiKeyRole<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Configures the entity using the <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public override void Configure(EntityTypeBuilder<IdentityApiKeyRole<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .ToTable(TableNames.IDENTITY_API_KEY_ROLE);

        builder
            .HasQueryFilter(x => x.ApiKey.IdentityUser.IsActive);

        builder
            .HasOne(x => x.ApiKey)
            .WithMany()
            .IsRequired();

        builder
            .HasOne(x => x.Role)
            .WithMany()
            .IsRequired();

        builder
            .HasIndex(x => new
            {
                x.ApiKeyId,
                x.RoleId
            })
            .IsUnique();
    }
}