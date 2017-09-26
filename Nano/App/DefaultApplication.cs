using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.App
{
    /// <inheritdoc />
    public class DefaultApplication : BaseApplication<IConfiguration>
    {
        /// <inheritdoc />
        public DefaultApplication(IConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

        }
    }
}