using System.Threading;
using System.Threading.Tasks;
using Nano.App.Startup;
using Nano.Data;

namespace Nano.Web.Hosting.Startup.Tasks
{
    /// <inheritdoc />
    public class InitializeDatabaseStartupTask : BaseStartupTask
    {
        private readonly BaseDbContext dbContext;

        /// <inheritdoc />
        public InitializeDatabaseStartupTask(StartupTaskContext startupTaskContext, BaseDbContext dbContext = null)
            : base(startupTaskContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public override async Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.StartupTaskContext
                .Increment();

            if (this.dbContext != null)
            {
                await this.dbContext
                    .EnsureCreatedAsync(cancellationToken);

                await this.dbContext
                    .EnsureMigratedAsync(cancellationToken);

                await this.dbContext
                    .EnsureIdentityAsync(cancellationToken);
            }

            this.StartupTaskContext
                .Decrement();
        }
    }
}