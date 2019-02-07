using Nano.Models.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Update Request.
    /// </summary>
    public class EditRequest : BaseRequestPost
    {
        /// <summary>
        /// Entity.
        /// </summary>
        public virtual IEntityUpdatable Entity { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditRequest()
        {
            this.Action = "edit";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Entity;
        }
    }
}