using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.App.Startup;
using Nano.Web.Hosting.Startup;

namespace Nano.Web.Hosting.HealthChecks
{
    /// <inheritdoc />
    public class StartupHealthCheck : IHealthCheck
    {
        private readonly StartupTaskContext taskContext;

        /// <inheritdoc />
        public StartupHealthCheck(StartupTaskContext taskContext)
        {
            this.taskContext = taskContext ?? throw new ArgumentNullException(nameof(taskContext));
        }

        /// <inheritdoc />
        public virtual Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));
            
            var result = this.taskContext.IsDone
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy();

            return Task.FromResult(result);
        }
    }
}