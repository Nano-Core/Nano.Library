using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveStyle : BaseCspDirectiveSimple, ICspDirectiveStyle
{
    public override string Name => "style-src";
}