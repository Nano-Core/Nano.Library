using System.Collections.Generic;
using Nano.App.Models.Interfaces;

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

        /// <summary>
        /// Get Query String Parameters.
        /// </summary>
        /// <returns></returns>
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            return new List<KeyValuePair<string, string>>();
        }
    }
}