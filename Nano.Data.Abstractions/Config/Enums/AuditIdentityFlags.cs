using System;

namespace Nano.Data.Abstractions.Config.Enums;

/// <summary>
/// Flags to control which ASP.NET Identity tables are audited in Nano.
/// </summary>
[Flags]
public enum AuditIdentityFlags
{
    /// <summary>
    /// No Identity tables are audited.
    /// </summary>
    None = 0,

    /// <summary>
    /// Audit IdentityUser model.
    /// </summary>
    User = 1 << 0,

    /// <summary>
    /// Audit IdentityUserRole model.
    /// </summary>
    UserRole = 1 << 1,

    /// <summary>
    /// Audit IdentityUserClaim model.
    /// </summary>
    UserClaim = 1 << 2,

    /// <summary>
    /// Audit IdentityUserLogin model.
    /// </summary>
    UserLogin = 1 << 3,

    /// <summary>
    /// Audit IdentityRole model.
    /// </summary>
    Role = 1 << 4,

    /// <summary>
    /// Audit IdentityRoleClaim model.
    /// </summary>
    RoleClaim = 1 << 5,

    /// <summary>
    /// Audit IdentityApiKey model.
    /// </summary>
    ApiKey = 1 << 6,

    /// <summary>
    /// Audit IdentityApiKeyClaim model.
    /// </summary>
    ApiKeyClaim = 1 << 7,

    /// <summary>
    /// Audit IdentityApiKeyRole model.
    /// </summary>
    ApiKeyRole = 1 << 8,

    /// <summary>
    /// Standard Identity models audited: User, UserRole, ApiKey and ApiKeyRole.
    /// </summary>
    Standard = User | UserRole | ApiKey | ApiKeyRole,

    /// <summary>
    /// Audit all Identity model.
    /// </summary>
    All = ~0
}