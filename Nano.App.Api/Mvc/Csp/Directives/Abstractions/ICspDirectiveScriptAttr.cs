namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveScriptAttr : ICspDirective
{
    void UnsafeInline();
    void Nonces(params string[] nonces);
    void Hashes(params string[] hashes);
}