using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nano.Config;
using Nano.Data.Interfaces;

namespace Nano.Data;

/// <inheritdoc />
public abstract class BaseDbContextFactory<TProvider, TContext> : IDesignTimeDbContextFactory<TContext>
    where TProvider : class, IDataProvider
    where TContext : DbContext
{
    /// <inheritdoc />
    public virtual TContext CreateDbContext(string[] args)
    {
        var configuration = ConfigManager.BuildConfiguration();

        var builder = new DbContextOptionsBuilder<TContext>();
        var dataOptions = configuration
            .GetSection(DataOptions.SectionName)
            .Get<DataOptions>() ?? new DataOptions();

        var provider = Activator.CreateInstance(typeof(TProvider), dataOptions) as TProvider;
        provider?.Configure(builder);

        var dbContext = Activator.CreateInstance(typeof(TContext), builder.Options, dataOptions) as TContext;

        if (dbContext == null)
        {
            throw new NullReferenceException(nameof(dbContext));
        }

        return dbContext;
    }
}