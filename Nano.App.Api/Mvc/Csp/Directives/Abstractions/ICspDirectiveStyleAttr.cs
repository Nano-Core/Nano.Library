namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveStyleAttr : ICspDirectiveSimple
{
    void UnsafeInline();
    void UnsafeHashes();
    void ReportSample();
}