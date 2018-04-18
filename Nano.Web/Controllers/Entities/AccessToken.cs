using System;

namespace Nano.Web.Controllers.Entities
{
    /// <summary>
    /// Access Token.
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// Token.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// ExpireAt.
        /// </summary>
        public virtual DateTime ExpireAt { get; set; }
    }
}