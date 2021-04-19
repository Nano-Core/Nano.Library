using System.Threading;
using System.Threading.Tasks;
using Nano.Data;

namespace Nano.App.Startup.Tasks
{
    /// <inheritdoc />
    public class InitializeApplicationStartupTask : BaseStartupTask
    {
        private readonly DefaultDbContext dbContext;

        /// <inheritdoc />
        public InitializeApplicationStartupTask(StartupTaskContext startupTaskContext, DefaultDbContext dbContext = null)
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