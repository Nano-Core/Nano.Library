using Nano.Models.Interfaces;

namespace Nano.Api.Responses
{
    /// <summary>
    /// Update Response.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
    public class UpdateResponse<TEntity> : BaseResponse
    {
        /// <summary>
        /// Entity.
        /// </summary>
        public virtual TEntity Entity { get; set; }
    }
}