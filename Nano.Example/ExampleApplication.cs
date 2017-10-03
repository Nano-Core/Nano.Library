using Microsoft.Extensions.Configuration;
using Nano.App;

namespace Nano.Example
{
    /// <inheritdoc />
    public class ExampleApplication : DefaultApplication
    {
        /// <inheritdoc />
        public ExampleApplication(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}