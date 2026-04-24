namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveScriptElem : ICspDirectiveSimple
{
    void UnsafeInline();
    void UnsafeEval();
    void UnsafeWasmEval();
    void TrustedTypesEval();
    void StrictDynamic();
    void Nonces(params string[] nonces);
    void Hashes(params string[] hashes);
    void UnsafeAllowRedirects();
    void InlineSpeculationRules();
    void ReportSample();
}