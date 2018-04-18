using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace Nano.Console.Hosting
{
    /// <summary>
    /// We can't reference real servers in this sample without creating a circular repo dependency.
    /// This fake server lets us at least run the code.
    /// </summary>
    public class ConsoleServer : IServer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="application"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// 
        /// </summary>
        public IFeatureCollection Features { get; } = new FeatureCollection();

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
    }
}