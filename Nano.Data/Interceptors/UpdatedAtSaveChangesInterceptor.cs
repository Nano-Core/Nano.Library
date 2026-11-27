using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Interceptors.Helpers;

namespace Nano.Data.Interceptors;

internal sealed class UpdatedAtSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StampModifiedEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        StampModifiedEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }


    private static void StampModifiedEntities(DbContext? dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var entries = dbContext.ChangeTracker
            .Entries()
            .Where(x => x.State == EntityState.Modified)
            .ToArray();

        foreach (var entry in entries)
        {
            UpdatedAtStamper.Stamp(entry);
        }
    }
}
