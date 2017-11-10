using Microsoft.Extensions.Configuration;
using Nano.Models.Types;

namespace Nano.Eventing
{
    /// <summary>
    /// Eventing Options.
    /// Populated from "Eventing" <see cref="IConfigurationSection"/>.
    /// </summary>
    public class EventingOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        internal static string SectionName => "Eventing";

        /// <summary>
        /// Host.
        /// </summary>
        public virtual string Host { get; set; }

        /// <summary>
        /// VHost.
        /// </summary>
        public virtual string VHost { get; set; } = "/";

        /// <summary>
        /// Port.
        /// </summary>
        public virtual ushort Port { get; set; } = 5672;

        /// <summary>
        /// Timeout, in seconds.
        /// </summary>
        public virtual ushort Timeout { get; set; } = 30;

        /// <summary>
        /// Use Ssl.
        /// </summary>
        public virtual bool UseSsl { get; set; } = false;

        /// <summary>
        /// Heartbeat, in seconds.
        /// Zero means no hearbeat requests.
        /// </summary>
        public virtual ushort Heartbeat { get; set; } = 0;

        /// <summary>
        /// Authentication Credential.
        /// </summary>
        public virtual AuthenticationCredential AuthenticationCredential { get; set; }
    }
}