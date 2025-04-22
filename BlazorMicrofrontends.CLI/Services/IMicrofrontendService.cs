namespace BlazorMicrofrontends.CLI.Services;

/// <summary>
/// Service for managing microfrontends
/// </summary>
public interface IMicrofrontendService
{
    /// <summary>
    /// Creates a new microfrontend
    /// </summary>
    /// <param name="templateName">Name of the template to use</param>
    /// <param name="name">Name of the microfrontend</param>
    /// <param name="outputDir">Directory to create the microfrontend in</param>
    /// <param name="options">Additional options for the microfrontend</param>
    Task<bool> CreateMicrofrontendAsync(string templateName, string name, string outputDir, Dictionary<string, string> options);
    
    /// <summary>
    /// Adds a microfrontend to an app shell
    /// </summary>
    /// <param name="appShellDir">Directory of the app shell</param>
    /// <param name="microfrontendPath">Path to the microfrontend to add</param>
    /// <param name="options">Additional options for adding the microfrontend</param>
    Task<bool> AddMicrofrontendToAppShellAsync(string appShellDir, string microfrontendPath, Dictionary<string, string> options);
    
    /// <summary>
    /// Removes a microfrontend from an app shell
    /// </summary>
    /// <param name="appShellDir">Directory of the app shell</param>
    /// <param name="microfrontendId">ID of the microfrontend to remove</param>
    Task<bool> RemoveMicrofrontendFromAppShellAsync(string appShellDir, string microfrontendId);
    
    /// <summary>
    /// Gets information about a microfrontend
    /// </summary>
    /// <param name="microfrontendPath">Path to the microfrontend</param>
    Task<Dictionary<string, string>> GetMicrofrontendInfoAsync(string microfrontendPath);
    
    /// <summary>
    /// Enables or disables a microfrontend in an app shell
    /// </summary>
    /// <param name="appShellDir">Directory of the app shell</param>
    /// <param name="microfrontendId">ID of the microfrontend</param>
    /// <param name="enabled">Whether to enable or disable the microfrontend</param>
    Task<bool> SetMicrofrontendEnabledAsync(string appShellDir, string microfrontendId, bool enabled);
    
    /// <summary>
    /// Publishes a microfrontend to a NuGet package or artifact repository
    /// </summary>
    /// <param name="microfrontendDir">Directory of the microfrontend</param>
    /// <param name="options">Publish options</param>
    Task<bool> PublishMicrofrontendAsync(string microfrontendDir, Dictionary<string, string> options);
} 