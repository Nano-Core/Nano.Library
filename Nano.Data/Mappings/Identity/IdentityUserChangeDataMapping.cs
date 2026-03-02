using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Consts;

namespace Nano.Data.Mappings.Identity;

/// <summary>
/// Configures the EF Core mapping for <see cref="IdentityUserChangeData{TIdentity}"/> entities.
/// Sets table name, query filters, and properties for tracking user change data.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityUserChangeDataMapping<TIdentity> : BaseEntityIdentityMapping<IdentityUserChangeData<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Configures the entity using the <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public override void Configure(EntityTypeBuilder<IdentityUserChangeData<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .ToTable(TableNames.IDENTITY_USER_CHANGE_DATA);

        builder
            .HasQueryFilter(x => x.IdentityUser.IsActive);

        builder
            .HasOne(x => x.IdentityUser)
            .WithOne();

        builder
            .Property(x => x.NewEmail)
            .HasMaxLength(256);

        builder
            .Property(x => x.NewPhoneNumber)
            .HasMaxLength(20);
    }
}