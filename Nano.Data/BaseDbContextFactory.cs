using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nano.Config;
using Nano.Data.Interfaces;
using Nano.Security;

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

        var securityOptions = configuration
            .GetSection(SecurityOptions.SectionName)
            .Get<SecurityOptions>() ?? new SecurityOptions();

        var provider = Activator.CreateInstance(typeof(TProvider), dataOptions) as TProvider;
        provider?.Configure(builder);

        if (Activator.CreateInstance(typeof(TContext), builder.Options, dataOptions, securityOptions) is not TContext dbContext)
        {
            throw new NullReferenceException(nameof(dbContext));
        }

        return dbContext;
    }
}