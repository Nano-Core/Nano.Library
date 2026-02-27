namespace Nano.App.Abstractions;

/// <summary>
/// Represents the contract for a Nano console application.
/// </summary>
public interface IConsoleApplication : IApplication<IConsoleApplication>
{
    /// <summary>
    /// Builds the application and finalizes configuration.
    /// Must be called before <see cref="IApplication.Run"/>.
    /// </summary>
    IConsoleApplication Build();
}