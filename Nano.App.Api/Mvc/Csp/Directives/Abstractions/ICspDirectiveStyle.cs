namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveStyle : ICspDirectiveSimple
{
    void UnsafeInline();
    void UnsafeHashes();
    void Nonces(params string[] nonces);
    void Hashes(params string[] hashes);
    void ReportSample();
}