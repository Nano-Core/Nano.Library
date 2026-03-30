using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Extensions;

namespace Nano.Data.Interceptors;

internal sealed class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateSoftDeletedEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateSoftDeletedEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }


    private static void UpdateSoftDeletedEntities(DbContext? dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var entityEntries = dbContext.ChangeTracker
            .Entries<IEntitySoftDeletable>()
            .Where(x => x.State == EntityState.Deleted);

        foreach (var entityEntry in entityEntries)
        {
            entityEntry.State = EntityState.Modified;
            entityEntry.Entity.IsDeleted = DateTimeOffset.UtcNow.GetEpochTime();
        }
    }
}