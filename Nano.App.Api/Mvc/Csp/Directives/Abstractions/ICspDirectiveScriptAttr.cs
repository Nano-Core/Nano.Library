namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveScriptAttr : ICspDirectiveSimple
{
    void UnsafeInline();
    void UnsafeHashes();
    void ReportSample();
}