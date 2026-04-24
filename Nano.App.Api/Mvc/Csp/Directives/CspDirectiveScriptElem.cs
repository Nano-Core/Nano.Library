using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveScriptElem : BaseCspDirectiveSimple, ICspDirectiveScriptElem
{
    public override string Name => "script-src-elem";

    public void UnsafeEval() => this.Values.Add("'unsafe-eval'");
    public void UnsafeWasmEval() => this.Values.Add("'wasm-unsafe-eval'");
    public void TrustedTypesEval() => this.Values.Add("'trusted-types-eval'");
    public void StrictDynamic() => this.Values.Add("'strict-dynamic'");
    public void UnsafeAllowRedirects() => this.Values.Add("'unsafe-allow-redirects'");
    public void InlineSpeculationRules() => this.Values.Add("'inline-speculation-rules'");
}