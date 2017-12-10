using Microsoft.Extensions.Configuration;
using Nano.App;

namespace NanoCore.Example
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