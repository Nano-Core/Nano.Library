using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Data.Extensions;

internal static class EntityEntryExtensions
{
    internal static string GetAuditKeyName(this EntityEntry entityEntry)
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        var primaryKey = entityEntry.Metadata
            .FindPrimaryKey();

        if (primaryKey == null)
        {
            throw new NullReferenceException(nameof(primaryKey));
        }

        var keyPropertyName = primaryKey.Properties
            .Select(x => entityEntry.Property(x.Name).Metadata.Name)
            .First();

        return keyPropertyName;
    }

    internal static TIdentity? GetAuditKeyValue<TIdentity>(this EntityEntry entityEntry)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        var primaryKey = entityEntry.Metadata
            .FindPrimaryKey();

        if (primaryKey == null)
        {
            throw new NullReferenceException(nameof(primaryKey));
        }

        var entityKey = primaryKey.Properties
            .Select(x => entityEntry.Property(x.Name).CurrentValue)
            .First();

        var entityType = entityEntry.Entity
            .GetType();

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

    internal static bool HasOriginalValues(this EntityEntry entityEntry)
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        return entityEntry.Properties
            .Any(x => !Equals(x.OriginalValue, x.CurrentValue));
    }
}