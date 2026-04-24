using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveStyleAttr : BaseCspDirectiveSimple, ICspDirectiveStyleAttr
{
    public override string Name => "style-src-attr";
}