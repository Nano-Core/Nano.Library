namespace Nano.Data.Abstractions.Identity.Authentication.Models;

///// <inheritdoc />
//public class ExternalProviderCustom() : BaseExternalProvider<ImplicitFlow>("Custom");

/// <summary>
/// External login provider using custom authentication.
/// </summary>
/// <typeparam name="TFlow">The type of authentication flow.</typeparam>
/// <param name="name"></param>
public class ExternalProviderCustom<TFlow>(string name) : BaseExternalProvider<TFlow>(name)
    where TFlow : BaseAuthFlow;