using Microsoft.Extensions.Configuration;

namespace Nano.App.Eventing.Options
{
    /// <summary>
    /// Eventing Options.
    /// Populatd from "Eventing" <see cref="IConfigurationSection"/>.
    /// </summary>
    public class EventingOptions
    {
        /// <summary>
        /// Provider.
        /// </summary>
        public virtual string Provider { get; set; }

        /// <summary>
        /// Host.
        /// </summary>
        public virtual string Host { get; set; }

        /// <summary>
        /// VHost.
        /// </summary>
        public virtual string VHost { get; set; }

        /// <summary>
        /// Port.
        /// </summary>
        public virtual int Port { get; set; }

        /// <summary>
        /// Ssl Port.
        /// </summary>
        public virtual int SslPort { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public virtual string Password { get; set; }
    }
}