namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveScript : ICspDirective
{
    void UnsafeInline();
    void UnsafeEval();
    void UnsafeHashes();
    void UnsafeWasmEval();
    void StrictDynamic();
    void Nonces(params string[] nonces);
    void Hashes(params string[] hashes);
    void UnsafeHashedAttributes();
    void UnsafeAllowRedirects();
    void RequireTrustedType();
    void RequireSri();
    void ReportSample();
}