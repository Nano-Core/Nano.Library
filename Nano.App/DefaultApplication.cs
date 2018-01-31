using Microsoft.Extensions.Configuration;

namespace Nano.App
{
    /// <inheritdoc />
    public class DefaultApplication : BaseApplication
    {
        /// <inheritdoc />
        public DefaultApplication(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}