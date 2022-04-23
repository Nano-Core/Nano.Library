using System.ComponentModel.DataAnnotations;

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
        /// Timeout In Seconds.
        /// </summary>
        public virtual int TimeoutInSeconds { get; set; } = 30;

        /// <summary>
        /// LogIn.
        /// </summary>
        public virtual LogInCredentials LogIn { get; set; }

        /// <summary>
        /// LogIn Credentials.
        /// </summary>
        public class LogInCredentials
        {
            /// <summary>
            /// Username.
            /// </summary>
            [Required]
            [MaxLength(256)]
            public virtual string Username { get; set; }

            /// <summary>
            /// Password.
            /// </summary>
            [Required]
            [MaxLength(256)]
            public virtual string Password { get; set; }
        }
    }
}