using System.Collections.Generic;

namespace Nano.App.Api.Mvc.Csp.Directives.Abstractions;

internal interface ICspDirective
{
    string Name { get; }

    List<string> Values { get; }
}