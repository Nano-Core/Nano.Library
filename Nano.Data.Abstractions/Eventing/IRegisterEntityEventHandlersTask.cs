using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions.Eventing;

/// <summary>
/// 
/// </summary>
public interface IRegisterEntityEventHandlersTask
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RegisterEntityEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}