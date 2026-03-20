using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Data.Extensions;

internal static class EntityEntryExtensions
{
    internal static string GetKeyName(this EntityEntry entityEntry)
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        var primaryKey = entityEntry.Metadata
            .FindPrimaryKey()!;

        var keyPropertyName = primaryKey.Properties
            .Select(x => entityEntry.Property(x.Name).Metadata.Name)
            .First();

        return keyPropertyName;
    }

    internal static TIdentity? GetKeyValue<TIdentity>(this EntityEntry entityEntry)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        var primaryKey = entityEntry.Metadata
            .FindPrimaryKey()!;

        var entityType = entityEntry.Entity
            .GetType();

        var entityKey = primaryKey.Properties
            .Select(x => entityEntry.Property(x.Name).CurrentValue)
            .First();

        if (entityType.IsGenericType)
        {
            var genericType = entityType
                .GetGenericTypeDefinition();

            if (genericType == typeof(IdentityUserLogin<>) || genericType == typeof(IdentityUserClaim<>) || genericType == typeof(IdentityRoleClaim<>))
            {
                entityKey = entityEntry.Entity switch
                {
                    IdentityUserLogin<TIdentity> userLogin => userLogin.UserId,
                    IdentityUserClaim<TIdentity> userClaim => userClaim.UserId,
                    IdentityRoleClaim<TIdentity> roleClaim => roleClaim.RoleId,
                    _ => entityKey
                };
            }
        }

        return (TIdentity?)entityKey;
    }
}