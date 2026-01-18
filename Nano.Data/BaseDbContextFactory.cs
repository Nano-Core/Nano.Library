using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nano.Common.Config;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Config;
using System;

namespace Nano.Data;

/// <inheritdoc />
public abstract class BaseDbContextFactory<TProvider, TContext> : IDesignTimeDbContextFactory<TContext>
    where TProvider : class, IDataProvider
    where TContext : DbContext
{
    /// <inheritdoc />
    public virtual TContext CreateDbContext(string[] args)
    {
        var configuration = ConfigManager.BuildConfiguration(Environments.Development);

        var builder = new DbContextOptionsBuilder<TContext>();

        var options = new DataOptions();

        configuration
            .GetSection(DataOptions.SectionName)
            .Bind(options);

        if (options == null)
        {
            throw new NullReferenceException(nameof(options));
        }

        TProvider.Configure(builder, options);

        var optionsMonitor = new StaticOptionsMonitor<DataOptions>(options);

        return Activator.CreateInstance(typeof(TContext), builder.Options, optionsMonitor, null) is not TContext dbContext
            ? throw new NullReferenceException(nameof(dbContext))
            : dbContext;
    }
}