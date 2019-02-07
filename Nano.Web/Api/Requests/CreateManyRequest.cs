using System.Collections.Generic;
using Nano.Models.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Create Many Request.
    /// </summary>
    public class CreateManyRequest : BaseRequestPost
    {
        /// <summary>
        /// Entities.
        /// </summary>
        public virtual IEnumerable<IEntityCreatable> Entities { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateManyRequest()
        {
            this.Action = "create/many";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Entities;
        }
    }
}