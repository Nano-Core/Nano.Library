using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Config.Enums;
using Nano.Data.Abstractions.Identity.Consts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data;

internal sealed class DbMigrationTask<TIdentity>(ILogger<DbMigrationTask<TIdentity>> logger, IOptionsMonitor<DataOptions> options, BaseDbContext<TIdentity> dbContext, RoleManager<IdentityRole<TIdentity>>? roleManager = null)
    : IDbMigrationTask
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ILogger<DbMigrationTask<TIdentity>> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOptionsMonitor<DataOptions> options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly BaseDbContext<TIdentity> dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly RoleManager<IdentityRole<TIdentity>>? roleManager = roleManager;

    public async Task MigrateAndSeedAsync(CancellationToken cancellationToken = default)
    {
        await this.EnsureCreatedAsync(cancellationToken);
        await this.EnsureMigratedAsync(cancellationToken);
        await this.EnsureIdentityAsync(cancellationToken);
    }


    private Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        if (this.options.CurrentValue.StartupAction != DatabaseStartupAction.Create)
        {
            return Task.CompletedTask;
        }

        this.logger
            .LogInformation("Creating Database at start-up.");

        return this.dbContext.Database
            .EnsureCreatedAsync(cancellationToken);
    }
    private async Task EnsureMigratedAsync(CancellationToken cancellationToken = default)
    {
        if (this.options.CurrentValue.StartupAction != DatabaseStartupAction.Migrate)
        {
            return;
        }

        this.logger
            .LogInformation("Applying Migrations at start-up.");

        const int MAX_RETRIES = 5;

        for (var i = 0; i < MAX_RETRIES; i++)
        {
            try
            {
                await this.dbContext.Database
                    .MigrateAsync(cancellationToken);
            }
            catch
            {
                await Task.Delay(2000, cancellationToken);
            }
        }
    }
    private async Task EnsureIdentityAsync(CancellationToken cancellationToken = default)
    {
        if (roleManager == null)
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

            if (exists)
            {
                continue;
            }

            var identityRole = new IdentityRole<TIdentity>(role);

            await roleManager
                .CreateAsync(identityRole);
        }

        await dbContext
            .SaveChangesAsync(cancellationToken);
    }
}