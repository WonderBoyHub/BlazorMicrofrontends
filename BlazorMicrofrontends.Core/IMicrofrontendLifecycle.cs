namespace BlazorMicrofrontends.Core;

/// <summary>
/// Interface for managing the lifecycle of microfrontend modules.
/// </summary>
public interface IMicrofrontendLifecycle
{
    /// <summary>
    /// Initializes all registered microfrontend modules.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitializeAllModulesAsync();
    
    /// <summary>
    /// Initializes a specific microfrontend module.
    /// </summary>
    /// <param name="moduleId">The ID of the module to initialize.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitializeModuleAsync(string moduleId);
    
    /// <summary>
    /// Gets a list of all registered modules.
    /// </summary>
    /// <returns>A collection of microfrontend modules.</returns>
    IEnumerable<IMicrofrontendModule> GetAllModules();
    
    /// <summary>
    /// Gets a specific module by its ID.
    /// </summary>
    /// <param name="moduleId">The ID of the module to retrieve.</param>
    /// <returns>The microfrontend module, or null if no module with the specified ID exists.</returns>
    IMicrofrontendModule? GetModule(string moduleId);
} 