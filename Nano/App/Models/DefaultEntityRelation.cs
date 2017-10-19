using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.Models.Interfaces;

namespace Nano.App.Models
{
    /// <inheritdoc />
    public class DefaultEntityRelation<TRelation> : DefaultEntity
        where TRelation : IEntityIdentity<Guid>
    {
        /// <summary>
        /// Relation.
        /// </summary>
        [Required]
        public virtual TRelation Relation { get; set; }

        /// <summary>
        /// Summary.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public virtual string Summary { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string Description { get; set; }
    }
}