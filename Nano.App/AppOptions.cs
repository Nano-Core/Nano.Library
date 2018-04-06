using System;
using Swashbuckle.AspNetCore.Swagger;

namespace Nano.App
{
    /// <summary>
    /// App Options.
    /// </summary>
    public class AppOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        public static string SectionName => "App";

        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; } = "Application";

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Terms Of Service.
        /// </summary>
        public virtual string TermsOfService { get; set; }

        /// <summary>
        /// Version.
        /// </summary>
        public virtual string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Contact.
        /// </summary>
        public virtual Contact Contact { get; set; } = new Contact();

        /// <summary>
        /// License.
        /// </summary>
        public virtual License License { get; set; } = new License();

        /// <summary>
        /// Hosting.
        /// </summary>
        public virtual HostingOptions Hosting { get; set; } = new HostingOptions();

        /// <summary>
        /// Cultures.
        /// </summary>
        public virtual CultureOptions Cultures { get; set; } = new CultureOptions();

        /// <summary>
        /// Hosting Options.
        /// </summary>
        public class HostingOptions
        {
            /// <summary>
            /// Root.
            /// </summary>
            public virtual string Root { get; set; } = "api";

            /// <summary>
            /// Ports.
            /// </summary>
            public virtual int[] Ports { get; set; } = new int[0];

            /// <summary>
            /// Use Ssl.
            /// </summary>
            public virtual bool UseSsl { get; set; }
        }

        /// <summary>
        /// Culture Options (nested class).
        /// </summary>
        public class CultureOptions
        {
            /// <summary>
            /// Default.
            /// </summary>
            public virtual string Default { get; set; } = "en-US";

            /// <summary>
            /// Supported.
            /// </summary>
            public virtual string[] Supported { get; set; } = new string[0];
        }

        /// <summary>
        /// Security Options.
        /// </summary>
        public class SecurityOptions
        {
            /// <summary>
            /// Is Enabled.
            /// </summary>
            public virtual bool IsEnabled { get; set; } = false;

            /// <summary>
            /// User Options.
            /// </summary>
            public virtual UserOptions User { get; set; } = new UserOptions();

            /// <summary>
            /// Password Options.
            /// </summary>
            public virtual PasswordOptions Password { get; set; } = new PasswordOptions();

            /// <summary>
            /// Sign In Options.
            /// </summary>
            public virtual SignInOptions SignIn { get; set; } = new SignInOptions();

            /// <summary>
            /// Lockout Options.
            /// </summary>
            public virtual LockoutOptions Lockout { get; set; } = new LockoutOptions();

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
}