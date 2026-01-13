using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Identity.Consts;
using Nano.Data.Mappings;

namespace Nano.Data.Identity.Mappings;

/// <inheritdoc />
public class IdentityUserRefreshTokenMapping<TIdentity> : BaseEntityIdentityMapping<IdentityUserRefreshToken<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<IdentityUserRefreshToken<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Map(builder);

        builder
            .ToTable(TableNames.IDENTITY_USER_REFRESH_TOKEN);

        builder
            .HasQueryFilter(x => x.IdentityUser.IsActive);

        builder
            .HasOne(x => x.IdentityUser)
            .WithOne();

        builder
            .Property(x => x.AppId)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .HasIndex(x => x.AppId);

        builder
            .Property(x => x.Value)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.ExpireAt);

        builder
            .HasIndex(x => x.ExpireAt);
    }
}