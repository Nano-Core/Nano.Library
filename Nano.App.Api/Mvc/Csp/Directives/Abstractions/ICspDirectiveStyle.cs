namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveStyle : ICspDirective
{
    void UnsafeInline();
    void UnsafeHashes();
    void Nonces(params string[] nonces);
    void Hashes(params string[] hashes);
    void RequireTrustedType();
    void RequireSri();
}