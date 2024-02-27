using Microsoft.Extensions.Logging;

namespace Nano.Security;

/// <summary>
/// Transient Identity Manager.
/// </summary>
public class TransientIdentityManager : BaseIdentityManager
{
    /// <inheritdoc />
    public TransientIdentityManager(ILogger logger, SecurityOptions options)
        : base(logger, options)
    {
    }
}