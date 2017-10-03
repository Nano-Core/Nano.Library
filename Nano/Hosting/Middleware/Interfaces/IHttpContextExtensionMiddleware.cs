using Microsoft.AspNetCore.Http;

namespace Nano.Hosting.Middleware.Interfaces
{
    /// <summary>
    /// Http Context Extension Middleware.
    /// Adds scoped http context logging and response handling for empty responses and errors.
    /// </summary>
    public interface IHttpContextExtensionMiddleware : IMiddleware
    {

    }
}