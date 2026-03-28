using Microsoft.AspNetCore.Routing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions;

internal interface IRegisterDataIdentityEndpoints
{
    Task RegisterEndpoints(IEndpointRouteBuilder builder, IServiceProvider serviceProvider, string root, CancellationToken cancellationToken = default);
}