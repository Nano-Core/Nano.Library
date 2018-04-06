using System;

namespace Nano.Security
{
    /// <summary>
    /// Security Options.
    /// </summary>
    public class SecurityOptions
    {        
        /// <summary>
        /// Section Name.
        /// </summary>
        public static string SectionName => "Security";

        /// <summary>
        /// Is Enabled.
        /// </summary>
        public virtual bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Jwt Options.
        /// </summary>
        public virtual JwtOptions Jwt { get; set; } = new JwtOptions();

        /// <summary>
        /// User Options.
        /// </summary>
        public virtual UserOptions User { get; set; } = new UserOptions();

        /// <summary>
        /// Sign In Options.
        /// </summary>
        public virtual SignInOptions SignIn { get; set; } = new SignInOptions();

        /// <summary>
        /// Lockout Options.
        /// </summary>
        public virtual LockoutOptions Lockout { get; set; } = new LockoutOptions();

        /// <summary>
        /// Password Options.
        /// </summary>
        public virtual PasswordOptions Password { get; set; } = new PasswordOptions();

        /// <summary>
        /// Jwt Options (nested class)
        /// </summary>
        public class JwtOptions
        {
            /// <summary>
            /// Issuer.
            /// </summary>
            public virtual string Issuer { get; set; }

            /// <summary>
            /// Secret Key.
            /// </summary>
            public virtual string SecretKey { get; set; }

            /// <summary>
            /// Expiration In Hours.
            /// </summary>
            public virtual int ExpirationInHours { get; set; } = 72;
        }

        /// <summary>
        /// User Options (nested class).
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
            public virtual string AllowedUserNameCharacters { get; set; }
        }

        /// <summary>
        /// Sign-In Options (nested class).
        /// </summary>
        public class SignInOptions
        {
            /// <summary>
            /// Require Confirmed Email-
            /// </summary>
            public virtual bool RequireConfirmedEmail { get; set; } = false;

            /// <summary>
            /// Require Confirmed PhoneNumber.
            /// </summary>
            public virtual bool RequireConfirmedPhoneNumber { get; set; } = false;
        }

        /// <summary>
        /// Lockout Options (nested class).
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
            public virtual int MaxFailedAccessAttempts { get; set; } = 3;

            /// <summary>
            /// Default Lockout TimeSpan.
            /// </summary>
            public virtual TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(30);
        }

        /// <summary>
        /// Password Options (nested class).
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
            public virtual int RequiredLength { get; set; } = 12;

            /// <summary>
            /// Required Unique Characters.
            /// </summary>
            public virtual int RequiredUniqueCharecters { get; set; } = 0;
        }
    }
}