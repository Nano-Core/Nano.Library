using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Remove Claim.
    /// </summary>
    /// <typeparam name="TIdentity">The identity key type</typeparam>
    public class RemoveClaim<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Id.
        /// </summary>
        [Required]
        public virtual TIdentity Id { get; set; }

        /// <summary>
        /// Claim Type.
        /// </summary>
        [Required]
        public virtual string ClaimType { get; set; }
    }
}