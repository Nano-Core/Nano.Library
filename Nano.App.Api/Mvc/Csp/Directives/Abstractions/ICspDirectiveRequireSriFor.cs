namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirectiveRequireSriFor : ICspDirective
{
    void Script();
    void Style();
}