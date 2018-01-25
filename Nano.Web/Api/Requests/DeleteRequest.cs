using System.Collections.Generic;
using Nano.Models.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Delete Request.
    /// </summary>
    public class DeleteRequest<TEntity> : BaseRequest
        where TEntity : IEntityDeletable
    {
        /// <summary>
        /// Entity.
        /// </summary>
        public virtual TEntity Entity { get; set; }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            return new List<KeyValuePair<string, string>>();
        }
    }
}