using System;

namespace Nano.Security.Exceptions
{
    /// <summary>
    /// Unauthorized Two-Factor Required Exception.
    /// </summary>
    public class UnauthorizedTwoFactorRequiredException : UnauthorizedAccessException
    {
        private const string CODE = "TwoFactorAuthenticationRequired";

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnauthorizedTwoFactorRequiredException()
            : base(UnauthorizedTwoFactorRequiredException.CODE)
        {

        }
    }
}