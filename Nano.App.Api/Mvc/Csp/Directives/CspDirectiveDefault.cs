using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveDefault : BaseCspDirectiveSimple, ICspDirectiveDefault
{
    public override string Name => "default-src";
}