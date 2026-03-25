namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents the base class for all authentication flows used with external providers.
/// This class can be extended to implement specific flows, such as authorization code or implicit flows.
/// </summary>
public abstract class BaseAuthFlow;