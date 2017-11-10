using Nano.Api;

namespace Nano.Example.Api
{
    /// <inheritdoc />
    public class ExamplesApi : BaseApi
    {
        /// <inheritdoc />
        public ExamplesApi() 
            : base(new ApiConnect
            {
                App = "Examples",
                Host = "127.0.0.1",
                Path = "app",
                Controller = "Examples",
                Port = 80
            })
        {
        }
    }
}