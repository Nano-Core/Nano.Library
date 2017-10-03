using System;
using Microsoft.Extensions.Configuration;

namespace Nano._Security.Options
{
    /// <summary>
    /// Security section.
    /// Populatd from security <see cref="IConfiguration"/> node.
    /// </summary>
    public class SecurityOptions
    {
        /// <summary>
        /// User Options.
        /// </summary>
        public virtual UserOptions User { get; set; }

        /// <summary>
        /// Password Options.
        /// </summary>
        public virtual PasswordOptions Password { get; set; }

        /// <summary>
        /// Sign In Options.
        /// </summary>
        public virtual SignInOptions SignIn { get; set; }

        /// <summary>
        /// Lockout Options.
        /// </summary>
        public virtual LockoutOptions Lockout { get; set; }

        /// <summary>
        /// User (nested class)
        /// </summary>
        public class UserOptions
        {
            /// <summary>
            /// Require Unique Email.
            /// </summary>
            public virtual bool RequireUniqueEmail { get; set; } = true;

            /// <summary>
            /// Allowed User Name Characters.
            /// </summary>
            public virtual string AllowedUserNameCharacters { get; set; } = null;
        }

        /// <summary>
        /// Password (nested class)
        /// </summary>
        public class PasswordOptions
        {
            /// <summary>
            /// Require Digit.
            /// </summary>
            public virtual bool RequireDigit { get; set; } = true;

            /// <summary>
            /// Require Non Alphanumeric.
            /// </summary>
            public virtual bool RequireNonAlphanumeric { get; set; } = true;

            /// <summary>
            /// Require Lowercase.
            /// </summary>
            public virtual bool RequireLowercase { get; set; } = true;

            /// <summary>
            /// Require Uppercase.
            /// </summary>
            public virtual bool RequireUppercase { get; set; } = true;

            /// <summary>
            /// Required Length.
            /// </summary>
            public virtual int RequiredLength { get; set; } = 16;

            /// <summary>
            /// Required Unique Characters.
            /// </summary>
            public virtual int RequiredUniqueCharecters { get; set; } = 0;
        }

        /// <summary>
        /// Sign In (nested class)
        /// </summary>
        public class SignInOptions
        {
            /// <summary>
            /// Require Confirmed Email-
            /// </summary>
            public virtual bool RequireConfirmedEmail { get; set; } = true;

            /// <summary>
            /// Require Confirmed PhoneNumber.
            /// </summary>
            public virtual bool RequireConfirmedPhoneNumber { get; set; } = true;
        }

        /// <summary>
        /// SignIn (nested class)
        /// </summary>
        public class LockoutOptions
        {
            /// <summary>
            /// Allowed For New Users.
            /// </summary>
            public virtual bool AllowedForNewUsers { get; set; } = true;

            /// <summary>
            /// Max Failed Access Attempts.
            /// </summary>
            public virtual int MaxFailedAccessAttempts { get; set; } = 10;

            /// <summary>
            /// Default Lockout TimeSpan.
            /// </summary>
            public virtual TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(30);
        }
    }
}