using Microsoft.Extensions.Configuration;
using Nano.App;

namespace Nano.Services.Globale
{
    /// <inheritdoc />
    public class GlobaleApplication : DefaultApplication
    {
        /// <inheritdoc />
        public GlobaleApplication(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}