using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Identity.Consts;
using Nano.Data.Mappings;

namespace Nano.Data.Identity.Mappings;

/// <inheritdoc />
public class IdentityUserChangeDataMapping<TIdentity> : BaseEntityIdentityMapping<IdentityUserChangeData<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<IdentityUserChangeData<TIdentity>> builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        base.Map(builder);

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