using Microsoft.AspNetCore.Routing;

namespace Nano.App.Api.Mvc.Authentication.Abstractions;

internal interface IRegisterTransientAuthEndpointsTask
{
    void RegisterEndpoints(IEndpointRouteBuilder builder, string version, string root);
}