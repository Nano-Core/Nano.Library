using Microsoft.Extensions.Configuration;

namespace Nano.Example
{
    /// <inheritdoc />
    public class Application : DefaultApplication
    {
        /// <inheritdoc />
        public Application(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}