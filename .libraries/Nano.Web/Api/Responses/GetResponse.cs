using Nano.Models.Interfaces;

namespace Nano.Web.Api.Responses
{
    /// <summary>
    /// Get Response.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
    public class GetResponse<TEntity> : BaseResponse
    {
        /// <summary>
        /// Entity.
        /// </summary>
        public virtual TEntity Entity { get; set; }
    }
}