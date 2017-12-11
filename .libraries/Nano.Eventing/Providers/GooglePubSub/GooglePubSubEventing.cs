using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Nano.Eventing.Interfaces;

namespace Nano.Eventing.Providers.GooglePubSub
{
    /// <inheritdoc />
    public class GooglePubSubEventing : IEventing
    {
        /// <summary>
        /// Project.
        /// </summary>
        protected virtual string ProjectName { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        public GooglePubSubEventing(string projectName)
        {
            if (projectName == null)
                throw new ArgumentNullException(nameof(projectName));

            this.ProjectName = projectName;
        }

        /// <inheritdoc />
        public virtual async Task Publish<TMessage>(TMessage body, string routing = "")
            where TMessage : class
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            await Task.Run(() => null);
        }

        /// <inheritdoc />
        public virtual async Task Subscribe<TMessage>(Action<TMessage> callback, string routing = "")
            where TMessage : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var name = typeof(TMessage).FullName;
            var subscriptionName = new SubscriptionName(this.ProjectName, name);

            var subscriber = await SimpleSubscriber.CreateAsync(subscriptionName);

            await subscriber.StartAsync((message, cancellationToken) =>
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                //var data = message.Data.ToString() as TMessage;
                //callback(data);

                return Task.FromResult(SimpleSubscriber.Reply.Ack);
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            
        }
    }
}