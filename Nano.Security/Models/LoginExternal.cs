using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login External.
    /// </summary>
    public class LoginExternal : LoginExternalProvider
    {
        /// <summary>
        /// App Id.
        /// </summary>
        [MaxLength(256)]
        public virtual string AppId { get; set; }

        /// <summary>
        /// Is Remember Me.
        /// </summary>
        [Required]
        [DefaultValue(false)]
        public virtual bool IsRememerMe { get; set; } = false;

        /// <summary>
        /// Is Refreshable.
        /// </summary>
        [Required]
        [DefaultValue(true)]
        public virtual bool IsRefreshable { get; set; } = true;

        /// <summary>
        /// Transient Claims.
        /// </summary>
        public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
    }
}