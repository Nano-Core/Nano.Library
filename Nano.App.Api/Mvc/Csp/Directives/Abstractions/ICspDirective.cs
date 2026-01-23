namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirective
{
    void None();
    void Self();
    void Sources(params string[] sources);
}