using System.Collections.Generic;
using Nano.Models.Interfaces;

namespace Nano.Web.Api.Responses
{
    /// <summary>
    /// Get All Response.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
    public class GetAllResponse<TEntity> : BaseResponse
    {
        /// <summary>
        /// Entities.
        /// </summary>
        public virtual ICollection<TEntity> Entities { get; set; }
    }
}