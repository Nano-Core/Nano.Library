using System;
using System.Threading.Tasks;
using Nano.Eventing.Interfaces;

namespace Nano.Eventing
{
    /// <summary>
    /// Null Eventing.
    /// </summary>
    public class NullEventing : IEventing
    {
        /// <inheritdoc />
        public virtual Task PublishAsync<TMessage>(TMessage body, string routing = "") 
            where TMessage : class
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task SubscribeAsync<TMessage>(IServiceProvider serviceProvider, string routing = "") 
            where TMessage : class
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {

        }
    }
}