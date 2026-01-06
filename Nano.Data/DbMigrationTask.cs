using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.Data;

/// <inheritdoc />
public class DbMigrationTask<TIdentity> : IDbMigrationTask
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ILogger logger;
    private readonly IOptionsMonitor<DataOptions> options;
    private readonly BaseDbContext<TIdentity> dbContext;
    private readonly RoleManager<IdentityRole<TIdentity>> roleManager;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="options">The <see cref="IOptionsMonitor{DataOptions}"/></param>
    /// <param name="dbContext">The <see cref="BaseDbContext{TIdentity}"/>.</param>
    /// <param name="roleManager">The <see cref="RoleManager{T}"/></param>
    public DbMigrationTask(ILogger logger, IOptionsMonitor<DataOptions> options, BaseDbContext<TIdentity> dbContext, RoleManager<IdentityRole<TIdentity>> roleManager = null)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.roleManager = roleManager;
    }

    /// <inheritdoc />
    public virtual async Task MigrateAndSeedAsync(CancellationToken cancellationToken = default)
    {
        await this.EnsureCreatedAsync(cancellationToken);
        await this.EnsureMigratedAsync(cancellationToken);
        await this.EnsureIdentityAsync(cancellationToken);
    }


    private async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        if (!this.options.CurrentValue.UseCreateDatabase)
        {
            return;
        }

        this.logger
            .LogInformation("Creating Database at start-up.");

        await this.dbContext.Database
            .EnsureCreatedAsync(cancellationToken);
    }
    private Task EnsureMigratedAsync(CancellationToken cancellationToken = default)
    {
        if (!this.options.CurrentValue.UseMigrateDatabase)
        {
            return Task.CompletedTask;
        }

        this.logger
            .LogInformation("Applying Migrations at start-up.");

        return this.dbContext.Database
            .MigrateAsync(cancellationToken);
    }
    private async Task EnsureIdentityAsync(CancellationToken cancellationToken = default)
    {
        if (this.roleManager == null)
        {
            return;
        }

        var roles = new[]
        {
            BuiltInUserRoles.READER,
            BuiltInUserRoles.WRITER,
            BuiltInUserRoles.CREATOR,
            BuiltInUserRoles.EDITOR,
            BuiltInUserRoles.DELETER,
            BuiltInUserRoles.IDENTITY,
            BuiltInUserRoles.ADMINISTRATOR
        };

        this.logger
            .LogInformation("Ensuring Identity Seed at start-up.");

        foreach (var role in roles)
        {
            var exists = await roleManager
                .RoleExistsAsync(role);

            if (!exists)
            {
                var identityRole = new IdentityRole<TIdentity>(role);

                await roleManager
                    .CreateAsync(identityRole);
            }
        }

        await dbContext
            .SaveChangesAsync(cancellationToken);
    }
}