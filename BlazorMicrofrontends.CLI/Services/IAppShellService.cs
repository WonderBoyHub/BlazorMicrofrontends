namespace BlazorMicrofrontends.CLI.Services;

/// <summary>
/// Service for managing Blazor microfrontend app shells
/// </summary>
public interface IAppShellService
{
    /// <summary>
    /// Creates a new app shell
    /// </summary>
    /// <param name="templateName">Name of the template to use</param>
    /// <param name="name">Name of the app shell</param>
    /// <param name="outputDir">Directory to create the app shell in</param>
    /// <param name="options">Additional options for the app shell</param>
    Task<bool> CreateAppShellAsync(string templateName, string name, string outputDir, Dictionary<string, string> options);
    
    /// <summary>
    /// Sets configuration for an app shell
    /// </summary>
    /// <param name="appShellDir">Directory of the app shell</param>
    /// <param name="options">Configuration options to set</param>
    Task<bool> ConfigureAppShellAsync(string appShellDir, Dictionary<string, string> options);
    
    /// <summary>
    /// Lists all microfrontends in an app shell
    /// </summary>
    /// <param name="appShellDir">Directory of the app shell</param>
    Task<IEnumerable<string>> ListMicrofrontendsAsync(string appShellDir);
    
    /// <summary>
    /// Publishes an app shell to a NuGet package or artifact repository
    /// </summary>
    /// <param name="appShellDir">Directory of the app shell</param>
    /// <param name="options">Publish options</param>
    Task<bool> PublishAppShellAsync(string appShellDir, Dictionary<string, string> options);
} 