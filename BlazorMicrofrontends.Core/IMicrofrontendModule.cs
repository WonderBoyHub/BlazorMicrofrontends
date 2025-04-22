using Microsoft.Extensions.DependencyInjection;

namespace BlazorMicrofrontends.Core;

/// <summary>
/// Represents a microfrontend module that can be registered and loaded dynamically.
/// </summary>
public interface IMicrofrontendModule
{
    /// <summary>
    /// Gets the unique identifier for this microfrontend module.
    /// </summary>
    string ModuleId { get; }
    
    /// <summary>
    /// Gets the display name of the microfrontend module.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the version of the microfrontend module.
    /// </summary>
    string Version { get; }
    
    /// <summary>
    /// Gets the technology/framework used by this microfrontend.
    /// </summary>
    string Technology { get; }
    
    /// <summary>
    /// Gets the routes supported by this microfrontend module.
    /// </summary>
    IReadOnlyList<(string Path, string Title)> Routes { get; }
    
    /// <summary>
    /// Configure the services needed by this microfrontend module.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    void ConfigureServices(IServiceCollection services);
    
    /// <summary>
    /// Initialize the microfrontend module.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitializeAsync();
    
    /// <summary>
    /// Gets whether the module has been initialized.
    /// </summary>
    bool IsInitialized { get; }
} 