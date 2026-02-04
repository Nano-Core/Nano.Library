namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveScript : ICspDirectiveSimple
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
    void ReportSample();
}