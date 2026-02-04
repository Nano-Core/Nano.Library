using System.Collections.Generic;
using Nano.App.Api.Mvc.Csp.Directives.Abstractions;

namespace Nano.App.Api.Mvc.Csp.Directives;

internal abstract class BaseCspDirective : ICspDirective
{
    public abstract string Name { get; }

    public List<string> Values { get; } = [];
}