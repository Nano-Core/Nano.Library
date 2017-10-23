using System.Collections.Generic;
using Nano.App.Models.Interfaces;

namespace Nano.Api.Responses
{
    /// <summary>
    /// Query Response.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
    public class QueryResponse<TEntity> : BaseResponse
    {
        /// <summary>
        /// Entities.
        /// </summary>
        public virtual IEnumerable<TEntity> Entities { get; set; }
    }
}