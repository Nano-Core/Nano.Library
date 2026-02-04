using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveStyleElem : BaseCspDirectiveSimple, ICspDirectiveStyleElem
{
    public override string Name => "style-src-elem";
}