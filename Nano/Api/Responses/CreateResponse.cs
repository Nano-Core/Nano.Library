using Nano.App.Models.Interfaces;

namespace Nano.Api.Responses
{
    /// <summary>
    /// Create Response.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
    public class CreateResponse<TEntity> : BaseResponse
    {
        /// <summary>
        /// Entity.
        /// </summary>
        public virtual TEntity Entity { get; set; }
    }
}