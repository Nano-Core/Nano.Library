namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveScriptElem : ICspDirective
{
    void Nonces(params string[] nonces);
    void Hashes(params string[] hashes);
}