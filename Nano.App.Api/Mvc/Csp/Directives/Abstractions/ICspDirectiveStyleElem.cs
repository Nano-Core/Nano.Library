namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveStyleElem : ICspDirectiveSimple
{
    void UnsafeInline();
    void Nonces(params string[] nonces);
    void Hashes(params string[] hashes);
    void ReportSample();
}