using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nano.Common.Config;
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

        var options = new DataOptions();

        configuration
            .GetSection(DataOptions.SectionName)
            .Bind(options);

        if (options == null)
        {
            throw new NullReferenceException(nameof(options));
        }

        if (Activator.CreateInstance(typeof(TProvider)) is not TProvider dataProvider)
        {
            throw new NullReferenceException(nameof(dataProvider));
        }

        dataProvider
            .Configure(builder, options);

        var optionsMonitor = new StaticOptionsMonitor<DataOptions>(options);

        if (Activator.CreateInstance(typeof(TContext), builder.Options, optionsMonitor, null) is not TContext dbContext)
        {
            throw new NullReferenceException(nameof(dbContext));
        }

        return dbContext;
    }
}