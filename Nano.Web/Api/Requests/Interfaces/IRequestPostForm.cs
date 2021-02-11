using System.Collections.Generic;
using Nano.Web.Api.Requests.Types;

namespace Nano.Web.Api.Requests.Interfaces
{
    /// <summary>
    /// Base interface for form requests (Multipart/form).
    /// </summary>
    public interface IRequestPostForm : IRequest
    {
        /// <summary>
        /// Get Body.
        /// </summary>
        IEnumerable<FormItem> GetForm();
    }
}