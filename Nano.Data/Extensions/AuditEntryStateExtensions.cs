using Nano.Data.Abstractions.Models.Enums;
using System;
using Z.EntityFramework.Plus;

namespace Nano.Data.Extensions;

internal static class AuditEntryStateExtensions
{
    internal static AuditState ToAuditState(this AuditEntryState state)
    {
        return state switch
        {
            AuditEntryState.EntityAdded => AuditState.Added,
            AuditEntryState.EntityDeleted => AuditState.Deleted,
            AuditEntryState.EntityModified => AuditState.Modified,
            AuditEntryState.EntitySoftAdded => AuditState.SoftAdded,
            AuditEntryState.EntitySoftDeleted => AuditState.SoftDeleted,
            AuditEntryState.RelationshipAdded => AuditState.RelationshipAdded,
            AuditEntryState.RelationshipDeleted => AuditState.RelationshipDeleted,
            AuditEntryState.EntityCurrent => AuditState.Current,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Argument of out range.")
        };
    }
}