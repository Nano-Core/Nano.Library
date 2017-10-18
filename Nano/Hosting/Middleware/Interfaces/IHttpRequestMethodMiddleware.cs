using Microsoft.AspNetCore.Http;

namespace Nano.Hosting.Middleware.Interfaces
{
    /// <summary>
    /// Http Request Method Middleware.
    /// Inspects the request http method, and handles 'options' method.
    /// </summary>
    public interface IHttpRequestMethodMiddleware : IMiddleware
    {

    }
}