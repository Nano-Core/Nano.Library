using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Eventing.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IRegisterEventHandlersTask
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RegisterEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}