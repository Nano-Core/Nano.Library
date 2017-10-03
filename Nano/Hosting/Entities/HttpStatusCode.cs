using System.Linq;
using Nano.Hosting.Entities.Collections;

namespace Nano.Hosting.Entities
{
    /// <summary>
    /// Status Code.
    /// </summary>
    public class HttpStatusCode
    {
        /// <summary>
        /// Code.
        /// </summary>
        public virtual int Code { get; set; }

        /// <summary>
        /// Summary.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets the <see cref="HttpStatusCode" /> mathcing the passed <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>The <see cref="HttpStatusCode"/>.</returns>
        public static HttpStatusCode Get(int statusCode)
        {
            return HttpStatusCodes.List.FirstOrDefault(x => x.Code == statusCode);
        }
    }
}