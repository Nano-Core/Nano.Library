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
        public Task Publish<TMessage>(TMessage body, string routing = "") 
            where TMessage : class
        {
            return Task.Run(() => { });
        }

        /// <inheritdoc />
        public Task Subscribe<TMessage>(Action<TMessage> callback, string routing = "") 
            where TMessage : class
        {
            return Task.Run(() => { });
        }

        /// <inheritdoc />
        public void Dispose()
        {

        }
    }
}