using BlazorMicrofrontends.Core;

namespace BlazorMicrofrontends.AppShell.Services;

/// <summary>
/// Defines methods for managing microfrontend modules in an application shell.
/// </summary>
public interface IMicrofrontendRegistry
{
    /// <summary>
    /// Gets all registered microfrontends.
    /// </summary>
    /// <returns>A collection of microfrontend modules.</returns>
    IEnumerable<IMicrofrontendModule> GetMicrofrontends();
    
    /// <summary>
    /// Gets a microfrontend module by its ID.
    /// </summary>
    /// <param name="moduleId">The ID of the microfrontend module.</param>
    /// <returns>The microfrontend module if found; otherwise, null.</returns>
    IMicrofrontendModule? GetMicrofrontend(string moduleId);
    
    /// <summary>
    /// Registers a microfrontend module.
    /// </summary>
    /// <param name="module">The microfrontend module to register.</param>
    void RegisterMicrofrontend(IMicrofrontendModule module);
    
    /// <summary>
    /// Unregisters a microfrontend module.
    /// </summary>
    /// <param name="moduleId">The ID of the microfrontend module to unregister.</param>
    void UnregisterMicrofrontend(string moduleId);
} 