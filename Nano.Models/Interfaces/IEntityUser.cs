using Microsoft.AspNetCore.Identity;

namespace Nano.Models.Interfaces
{
    /// <summary>
    /// Entity User inteface.
    /// </summary>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public interface IEntityUser<TIdentity> : IEntityIdentity<TIdentity>
    {
        /// <summary>
        /// Identity User Id.
        /// </summary>
        string IdentityUserId { get; set; }

        /// <summary>
        /// Identity User.
        /// </summary>
        IdentityUser IdentityUser { get; set; }
    }
}