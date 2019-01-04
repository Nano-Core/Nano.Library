using Nano.Models.Auth;

namespace Nano.Web.Api
{
    /// <summary>
    /// Api Options.
    /// </summary>
    public class ApiOptions
    {
        /// <summary>
        /// Host.
        /// </summary>
        public virtual string Host { get; set; } = "localhost";

        /// <summary>
        /// Root.
        /// </summary>
        public virtual string Root { get; set; } = "api";

        /// <summary>
        /// Port.
        /// </summary>
        public virtual int Port { get; set; } = 80;

        /// <summary>
        /// Use Ssl.
        /// </summary>
        public virtual bool UseSsl { get; set; } = false;

        /// <summary>
        /// Use Expose Erros.
        /// </summary>
        public virtual bool UseExposeErrors { get; set; } = false;

        /// <summary>
        /// Login.
        /// </summary>
        public virtual Login Login { get; set; }
    }
}