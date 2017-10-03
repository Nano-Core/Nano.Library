using Microsoft.AspNetCore.Http;

namespace Nano.Hosting.Middleware.Interfaces
{
    /// <summary>
    /// Http Request Identifier Middleware.
    /// Adds a 'RequestId' to response all headers.
    /// </summary>
    public interface IHttpRequestIdentifierMiddleware : IMiddleware
    {
        
    }
}