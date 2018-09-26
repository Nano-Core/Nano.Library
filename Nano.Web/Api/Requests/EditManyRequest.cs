using System.Collections.Generic;
using Nano.Models.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Update Many Request.
    /// </summary>
    public class EditManyRequest : BaseRequestJson
    {
        /// <summary>
        /// Entities.
        /// </summary>
        public virtual IEnumerable<IEntityUpdatable> Entities { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditManyRequest()
        {
            this.Action = "edit/many";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Entities;
        }
    }
}