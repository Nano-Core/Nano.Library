namespace Nano.Web.Api
{
    /// <summary>
    /// Api Connect.
    /// Uri Template: http(s)://{this.Host}:{this.Port}{this.Path}{this.App}{this.Controller}{this.Action}
    /// </summary>
    public class ApiConnect
    {
        /// <summary>
        /// App.
        /// </summary>
        public virtual string App { get; set; }

        /// <summary>
        /// Host.
        /// </summary>
        public virtual string Host { get; set; }

        /// <summary>
        /// Path.
        /// </summary>
        public virtual string Path { get; set; } = "api";

        /// <summary>
        /// Action.
        /// </summary>
        public virtual string Action { get; set; }

        /// <summary>
        /// Controller.
        /// </summary>
        public virtual string Controller { get; set; }

        /// <summary>
        /// Port.
        /// </summary>
        public virtual int Port { get; set; } = 80;

        /// <summary>
        /// Use Ssl.
        /// </summary>
        public virtual bool UseSsl { get; set; } = false;

        /// <summary>
        /// Absolute Url.
        /// </summary>
        public virtual string AbsoluteUrl
        {
            get
            {
                var protocol = this.UseSsl ? "https://" : "http://";
                return $"{protocol}{this.Host}:{this.Port}/{this.Path}/{this.App}/{this.Controller}/{this.Action}";
            }
        }
    }
}