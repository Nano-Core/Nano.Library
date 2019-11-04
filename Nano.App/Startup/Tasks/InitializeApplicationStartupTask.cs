using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Startup.Tasks
{
    /// <inheritdoc />
    public class InitializeApplicationStartupTask : BaseStartupTask
    {
        /// <inheritdoc />
        public InitializeApplicationStartupTask(StartupTaskContext startupTaskContext)
            : base(startupTaskContext)
        {

        }

        /// <inheritdoc />
        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.StartupTaskContext
                .Increment();

            this.StartupTaskContext
                .Decrement();

            return Task.CompletedTask;
        }
    }
}