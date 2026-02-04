using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveScript : BaseCspDirectiveSimple, ICspDirectiveScript
{
    public override string Name => "script-src";

    public void UnsafeEval() => this.Values.Add("'unsafe-eval'");
    public void UnsafeWasmEval() => this.Values.Add("'wasm-unsafe-eval'");
    public void StrictDynamic() => this.Values.Add("'strict-dynamic'");
    public void UnsafeHashedAttributes() => this.Values.Add("'unsafe-hashed-attributes'");
    public void UnsafeAllowRedirects() => this.Values.Add("'unsafe-allow-redirects'");
}