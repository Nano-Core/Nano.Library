using Microsoft.AspNetCore.Routing;
using System;

namespace Nano.Data.Abstractions.Identity;

/// <summary>
/// Defines a contract for tasks that register data identity–related endpoints
/// on an <see cref="IEndpointRouteBuilder"/>.
/// </summary>
public interface IRegisterDataIdentityEndpointsTask
{
    /// <summary>
    /// Registers data identity endpoints using the specified route builder, root path, and found user types.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to define the endpoints.</param>
    /// <param name="version">The default version of the application.</param>
    /// <param name="root">The root route prefix under which the identity endpoints will be registered.</param>
    /// <param name="userTypes">A collection of user entity types for which endpoints should be generated.</param>
    /// <param name="hasAuth">Whether authentication has been configured and the BaseAuthController has been implemented.</param>
    void RegisterEndpoints(IEndpointRouteBuilder builder, string version, string root, Type[] userTypes, bool hasAuth);
}