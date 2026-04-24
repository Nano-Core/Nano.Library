using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.App.Api.Mvc.Authentication.Abstractions;

/// <summary>
/// Marker interface to indicate that an <see cref="IAuthExternalRepository"/> implementation
/// is a built-in provider (e.g., Facebook, Google, Microsoft) and should be excluded from
/// automatic consumer registration.
/// </summary>
internal interface IBuiltInAuthExternalRepository;