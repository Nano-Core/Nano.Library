using System;

namespace Nano.Security.Exceptions
{
    /// <summary>
    /// Unauthorized Locked Out Exception.
    /// </summary>
    public class UnauthorizedLockedOutException : UnauthorizedAccessException
    {
        private const string CODE = "LockedOut";

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnauthorizedLockedOutException()
            : base(UnauthorizedLockedOutException.CODE)
        {

        }
    }
}
