using System.Collections.Generic;
using Nano.Api.Requests.Interfaces;
using Nano.App.Models.Interfaces;

namespace Nano.Api.Requests
{
    /// <summary>
    /// Update Request.
    /// </summary>
    public class UpdateRequest<TEntity> : BaseRequest, IRequestJson
        where TEntity : IEntityUpdatable
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