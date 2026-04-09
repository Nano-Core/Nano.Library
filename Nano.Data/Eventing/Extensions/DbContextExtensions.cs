//using System;
//using System.Collections;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;

//namespace Nano.Data.Eventing.Extensions;

//// BUG: REMOVE: probably remove these. check

//internal static class DbContextExtensions
//{
//    internal static object GetSetInstance(this DbContext dbContext, Type entityType)
//    {
//        ArgumentNullException.ThrowIfNull(dbContext);
//        ArgumentNullException.ThrowIfNull(entityType);

//        var method = typeof(DbContext)
//            .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
//            .MakeGenericMethod(entityType);

//        return method.Invoke(dbContext, null)!;
//    }

//    internal static IEnumerable GetLocalEntries(this DbContext dbContext, Type entityType)
//    {
//        ArgumentNullException.ThrowIfNull(dbContext);
//        ArgumentNullException.ThrowIfNull(entityType);

//        return dbContext.ChangeTracker.Entries()
//            .Where(e => e.Metadata.ClrType == entityType)
//            .Select(e => e.Entity);

//        //var dbSet = dbContext
//        //    .GetSetInstance(entityType);

//        //var localProperty = dbSet
//        //    .GetType()
//        //    .GetProperty("Local") ?? throw new InvalidOperationException("Local property not found.");

//        //return (IEnumerable)localProperty
//        //    .GetValue(dbSet)!;
//    }
//}