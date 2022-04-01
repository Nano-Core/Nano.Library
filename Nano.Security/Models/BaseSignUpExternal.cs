using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Base Sign-Up External (abstract).
    /// </summary>
    public abstract class BaseSignUpExternal : BaseSignUp
    {

    }

    /// <summary>
    /// Base Sign-Up External (abstract).
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    public abstract class BaseSignUpExternal<TProvider> : BaseSignUpExternal
        where TProvider : BaseLoginExternalProvider, new()
    {
        /// <summary>
        /// Login External.
        /// </summary>
        [Required]
        public virtual TProvider Provider { get; set; }
    }
}