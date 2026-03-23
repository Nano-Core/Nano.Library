using Microsoft.AspNetCore.Authentication;

namespace Nano.App.Api.Mvc.Authentication;

internal class AuthenticationSchemeCache
{
    internal AuthenticationScheme[] Schemes { get; set; } = [];
}