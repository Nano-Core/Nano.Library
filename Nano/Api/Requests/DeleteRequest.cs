using System.Collections.Generic;
using Nano.Api.Requests.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Api.Requests
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

        /// <inheritdoc cref="IRequest.GetQueryStringParameters()"/>
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            return new List<KeyValuePair<string, string>>();
        }
    }
}