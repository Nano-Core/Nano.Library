using System;
using Microsoft.AspNetCore.Identity;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Enums;
using Z.EntityFramework.Plus;

namespace Nano.Data.Extensions;

internal static class AuditEntryExtensions
{
    internal static TIdentity GetEntityKey<TIdentity>(this AuditEntry auditEntry)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(auditEntry);

        var entityKey = auditEntry.Entry
            .GetKeyValue<TIdentity>();

        var entityType = auditEntry.Entry.Entity
            .GetType();

        if (entityType.IsGenericType)
        {
            var genericType = entityType
                .GetGenericTypeDefinition();

            if (genericType == typeof(IdentityUserLogin<>) || genericType == typeof(IdentityUserClaim<>) || genericType == typeof(IdentityRoleClaim<>))
            {
                entityKey = auditEntry.Entry.Entity switch
                {
                    IdentityUserLogin<TIdentity> userLogin => userLogin.UserId,
                    IdentityUserClaim<TIdentity> userClaim => userClaim.UserId,
                    IdentityRoleClaim<TIdentity> roleClaim => roleClaim.RoleId,
                    _ => entityKey
                };
            }
        }

        return entityKey!;
    }

    internal static AuditState GetEntityState(this AuditEntry auditEntry)
    {
        ArgumentNullException.ThrowIfNull(auditEntry);

        return auditEntry is { State: AuditEntryState.EntityDeleted, Entity: IEntitySoftDeletable { IsDeleted: > 0L } }
            ? AuditState.SoftDeleted
            : auditEntry.State.ToAuditState();
    }

    internal static string GetEntityTypeName(this AuditEntry auditEntry)
    {
        ArgumentNullException.ThrowIfNull(auditEntry);

        return auditEntry.Entity
            .GetType()
            .GetFriendlyName();
    }
}