using System;
using System.Collections.Generic;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Specifies options for the <see cref="RequestTimeZoneMiddleware"/>.
    /// </summary>
    public class RequestTimeZoneOptions
    {
        private RequestTimeZone defaultRequestTimeZone = new RequestTimeZone("Etc/UTC");

        /// <summary>
        /// Gets or sets the default timezone to use for requests.
        /// </summary>
        public virtual RequestTimeZone DefaultRequestTimeZone
        {
            get
            {
                return this.defaultRequestTimeZone;
            }
            set
            {
                this.defaultRequestTimeZone = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// An ordered list of providers used to determine a request's timezone information.
        /// The first provider that returns a non-null result for a given request will be used.
        /// Defaults to the following:
        /// <list type="number">
        ///     <item><description><see cref="QueryStringRequestTimeZoneProvider"/></description></item>
        ///     <item><description><see cref="HeaderRequestTimeZoneProvider"/></description></item>
        ///     <item><description><see cref="CookieRequestTimeZoneProvider"/></description></item>
        /// </list>
        /// </summary>
        public virtual IList<IRequestTimeZoneProvider> RequestTimeZoneProviders { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RequestTimeZoneOptions()
        {
            this.RequestTimeZoneProviders = new List<IRequestTimeZoneProvider>
            {
                new QueryStringRequestTimeZoneProvider { Options = this },
                new HeaderRequestTimeZoneProvider { Options = this },
                new CookieRequestTimeZoneProvider { Options = this }
            };
        }
    }
}