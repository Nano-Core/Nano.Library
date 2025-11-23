using System.Threading;
using System.Threading.Tasks;
using Nano.Common.Startup;
using Nano.Common.Startup.Tasks;

namespace Nano.Data;

/// <inheritdoc />
public class MigrateDatabaseStartupTask : BaseStartupTask
{
    private readonly DefaultDbContext dbContext;

    /// <inheritdoc />
    public MigrateDatabaseStartupTask(StartupTaskContext startupTaskContext, DefaultDbContext dbContext = null)
        : base(startupTaskContext)
    {
        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        this.Context
            .Increment();

        await Task.CompletedTask;
        if (this.dbContext != null)
        {
            await this.dbContext
                .EnsureCreatedAsync(cancellationToken);

            await this.dbContext
                .EnsureMigratedAsync(cancellationToken);

            await this.dbContext
                .EnsureIdentityAsync(cancellationToken);
        }

        this.Context
            .Decrement();
    }
}