using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nano.Common.Config.Helpers;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;

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

        var dataOptions = new DataOptions();

        configuration
            .GetSection(DataOptions.SectionName)
            .Bind(dataOptions);

        if (dataOptions == null)
        {
            throw new NullReferenceException(nameof(dataOptions));
        }

        if (Activator.CreateInstance(typeof(TProvider), dataOptions) is not TProvider dataProvider)
        {
            throw new NullReferenceException(nameof(dataProvider));
        }

        dataProvider
            .Configure(builder);

        if (Activator.CreateInstance(typeof(TContext), builder.Options, dataOptions) is not TContext dbContext)
        {
            throw new NullReferenceException(nameof(dbContext));
        }

        return dbContext;
    }
}