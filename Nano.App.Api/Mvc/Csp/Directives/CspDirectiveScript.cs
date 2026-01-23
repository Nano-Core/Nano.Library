using System.Collections.Generic;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal sealed class CspDirectiveScript(List<string> values)
    : BaseCspBuilder(values), ICspDirectiveScript
{
    public void UnsafeEval() => this.values.Add("'unsafe-eval'");
    public void UnsafeWasmEval() => this.values.Add("'wasm-unsafe-eval'");
    public void StrictDynamic() => this.values.Add("'strict-dynamic'");
    public void UnsafeHashes() => this.values.Add("'unsafe-hashes'");
    public void UnsafeHashedAttributes() => this.values.Add("'unsafe-hashed-attributes'");
    public void UnsafeAllowRedirects() => this.values.Add("'unsafe-allow-redirects'");
    public void RequireTrustedType() => this.values.Add("'require-trusted-types-for script'");
    public void RequireSri() => this.values.Add("'require-sri-for script'");
    public void ReportSample() => this.values.Add("'report-sample'");
}