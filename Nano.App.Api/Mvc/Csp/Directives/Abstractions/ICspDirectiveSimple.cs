namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveSimple : ICspDirective
{
    void None();
    void Self();
    void Sources(params string[] sources);
}