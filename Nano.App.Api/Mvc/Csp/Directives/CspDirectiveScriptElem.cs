using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveScriptElem : BaseCspDirectiveSimple, ICspDirectiveScriptElem
{
    public override string Name => "script-src-elem";
}