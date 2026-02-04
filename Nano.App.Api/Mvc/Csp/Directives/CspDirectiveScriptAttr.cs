using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveScriptAttr : BaseCspDirectiveSimple, ICspDirectiveScriptAttr
{
    public override string Name => "script-src-attr";
}