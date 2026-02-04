namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveTrustedTypes
{
    void None();
    void TrustedTypes(params string[] policies);
    void AllowDuplicates();
}