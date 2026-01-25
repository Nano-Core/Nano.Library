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

/// <summary>
/// Base factory for creating <see cref="DbContext"/> instances at design-time for migrations.
/// </summary>
/// <typeparam name="TProvider">The type of <see cref="IDataProvider"/> used to configure the context.</typeparam>
/// <typeparam name="TContext">The type of <see cref="DbContext"/> to create.</typeparam>
public abstract class BaseDbContextFactory<TProvider, TContext> : IDesignTimeDbContextFactory<TContext>
    where TProvider : class, IDataProvider
    where TContext : DbContext
{
    /// <summary>
    /// Creates a new instance of the <typeparamref name="TContext"/> <see cref="DbContext"/> with configured options.
    /// </summary>
    /// <param name="args">Optional arguments passed by the design-time tools.</param>
    /// <returns>An instance of <typeparamref name="TContext"/>.</returns>
    /// <exception cref="NullReferenceException">Thrown if the <see cref="DataOptions"/> or the created <typeparamref name="TContext"/> instance is null.</exception>
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