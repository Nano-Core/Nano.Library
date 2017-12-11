using System;
using Nano.Eventing.Interfaces;

namespace Nano.Eventing.Providers.GooglePubSub
{
    /// <summary>
    /// Google PubSub Provider.
    /// </summary>
    public class GooglePubSubProvider : IEventingProvider
    {
        /// <summary>
        /// Options.
        /// </summary>
        protected virtual EventingOptions Options { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="EventingOptions"/>.</param>
        public GooglePubSubProvider(EventingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.Options = options;
        }

        /// <inheritdoc />
        public virtual IEventing Configure()
        {
            var project = this.Options.Host;

            return new GooglePubSubEventing(project);
        }
    }
}