using Microsoft.AspNetCore.Http;

namespace Nano.Hosting.Middleware.Interfaces
{
    /// <summary>
    /// Http Request Content Type Middleware.
    /// Adds support for Json, Xml and Html Content-Types for http requests and responses.
    /// </summary>
    public interface IHttpRequestContentTypeMiddleware : IMiddleware
    {

    }
}