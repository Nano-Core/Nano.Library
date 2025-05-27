using System;
using Nano.Security.Const;

namespace Nano.Security;

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
    /// Tokens Expiration.
    /// </summary>
    public virtual int TokensExpirationInHours { get; set; } = 24;

    /// <summary>
    /// Jwt Options.
    /// </summary>
    public virtual JwtOptions Jwt { get; set; } = new();

    /// <summary>
    /// Api Key.
    /// </summary>
    public virtual ApiKeyOptions ApiKey { get; set; } = new();

    /// <summary>
    /// User Options.
    /// </summary>
    public virtual UserOptions User { get; set; } = new();

    /// <summary>
    /// Sign In Options.
    /// </summary>
    public virtual SignInOptions SignIn { get; set; } = new();

    /// <summary>
    /// Lockout Options.
    /// </summary>
    public virtual LockoutOptions Lockout { get; set; } = new();

    /// <summary>
    /// Password Options.
    /// </summary>
    public virtual PasswordOptions Password { get; set; } = new();

    /// <summary>
    /// External Logins.
    /// </summary>
    public virtual ExternalLoginOptions ExternalLogins { get; set; } = new();

    /// <summary>
    /// Jwt Options (nested class)
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// Is Enabled.
        /// </summary>
        public virtual bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Issuer.
        /// </summary>
        public virtual string Issuer { get; set; } = "issuer";

        /// <summary>
        /// Audience.
        /// </summary>
        public virtual string Audience { get; set; } = "audience";

        /// <summary>
        /// Public Key.
        /// Base64 encoded.
        /// </summary>
        public virtual string PublicKey { get; set; } = "verysecretkey";

        /// <summary>
        /// Private Key.
        /// Base64 encoded.
        /// </summary>
        public virtual string PrivateKey { get; set; } = "veryprivatesecretkey";

        /// <summary>
        /// Expiration In Minutes.
        /// </summary>
        public virtual int ExpirationInMinutes { get; set; } = 60;

        /// <summary>
        /// Refresh Expiration In Hours.
        /// </summary>
        public virtual int RefreshExpirationInHours { get; set; } = 72;
    }

    /// <summary>
    /// Api Key Options (nested class)
    /// </summary>
    public class ApiKeyOptions
    {
        /// <summary>
        /// Is Enabled.
        /// </summary>
        public virtual bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Secret.
        /// </summary>
        public virtual string Secret { get; set; } = null;
    }

    /// <summary>
    /// User Options (nested class).
    /// </summary>
    public class UserOptions
    {
        /// <summary>
        /// Is Unique Email Address Required.
        /// </summary>
        public virtual bool IsUniqueEmailAddressRequired { get; set; } = true;

        /// <summary>
        /// Is Unique Phone Number Required.
        /// </summary>
        public virtual bool IsUniquePhoneNumberRequired { get; set; } = false;

        /// <summary>
        /// Allowed User Name Characters.
        /// Defaults to abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+
        /// </summary>
        public virtual string AllowedUserNameCharacters { get; set; } = null;

        /// <summary>
        /// Admin Password.
        /// </summary>
        public virtual string AdminPassword { get; set; }

        /// <summary>
        /// Admin Email Address.
        /// </summary>
        public virtual string AdminEmailAddress { get; set; }

        /// <summary>
        /// Default Roles.
        /// </summary>
        public virtual string[] DefaultRoles { get; set; } =
        [
            BuiltInUserRoles.READER,
            BuiltInUserRoles.WRITER,
            BuiltInUserRoles.SERVICE
        ];
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
        public virtual bool RequireDigit { get; set; } = false;

        /// <summary>
        /// Require Non Alphanumeric.
        /// </summary>
        public virtual bool RequireNonAlphanumeric { get; set; } = false;

        /// <summary>
        /// Require Lowercase.
        /// </summary>
        public virtual bool RequireLowercase { get; set; } = false;

        /// <summary>
        /// Require Uppercase.
        /// </summary>
        public virtual bool RequireUppercase { get; set; } = false;

        /// <summary>
        /// Required Length.
        /// </summary>
        public virtual int RequiredLength { get; set; } = 5;

        /// <summary>
        /// Required Unique Characters.
        /// </summary>
        public virtual int RequiredUniqueCharacters { get; set; } = 0;
    }

    /// <summary>
    /// External Login Options.
    /// </summary>
    public class ExternalLoginOptions
    {
        /// <summary>
        /// Google.
        /// </summary>
        public virtual GoogleOptions Google { get; set; }

        /// <summary>
        /// Facebook.
        /// </summary>
        public virtual FacebookOptions Facebook { get; set; }

        /// <summary>
        /// Microsoft.
        /// </summary>
        public virtual MicrosoftOptions Microsoft { get; set; }

        /// <summary>
        /// Google Options.
        /// </summary>
        public class GoogleOptions
        {
            /// <summary>
            /// Client Id.
            /// </summary>
            public virtual string ClientId { get; set; }

            /// <summary>
            /// Client Secret.
            /// </summary>
            public virtual string ClientSecret { get; set; }

            /// <summary>
            /// Scopes.
            /// </summary>
            public virtual string[] Scopes { get; set; } = [];
        }

        /// <summary>
        /// Facebook Options.
        /// </summary>
        public class FacebookOptions
        {
            /// <summary>
            /// App Id.
            /// </summary>
            public virtual string AppId { get; set; }

            /// <summary>
            /// App Secret.
            /// </summary>
            public virtual string AppSecret { get; set; }

            /// <summary>
            /// Scopes.
            /// </summary>
            public virtual string[] Scopes { get; set; } = [];
        }

        /// <summary>
        /// Microsoft Options.
        /// </summary>
        public class MicrosoftOptions
        {
            /// <summary>
            /// Tenant Id.
            /// </summary>
            public virtual string TenantId { get; set; }

            /// <summary>
            /// Client Id.
            /// </summary>
            public virtual string ClientId { get; set; }

            /// <summary>
            /// Client Secret.
            /// </summary>
            public virtual string ClientSecret { get; set; }

            /// <summary>
            /// Scopes.
            /// </summary>
            public virtual string[] Scopes { get; set; } = [];
        }
    }
}