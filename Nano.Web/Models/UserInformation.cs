using System;

namespace Nano.Web.Models;

/// <summary>
/// User Information.
/// </summary>
public class UserInformation
{
    /// <summary>
    /// Id.
    /// </summary>
    public virtual Guid? Id { get; set; }

    /// <summary>
    /// App Id.
    /// </summary>
    public virtual string AppId { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Email.
    /// </summary>
    public virtual string Email { get; set; }

    /// <summary>
    /// Time Zone.
    /// </summary>
    public virtual string TimeZone { get; set; }

    /// <summary>
    /// Culture.
    /// </summary>
    public virtual string Culture { get; set; }

    /// <summary>
    /// Jwt Token.
    /// </summary>
    public virtual string JwtToken { get; set; }

    /// <summary>
    /// Is Logged In
    /// </summary>
    public bool IsLoggedIn => this.Id.HasValue;
}