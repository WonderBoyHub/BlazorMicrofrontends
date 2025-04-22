using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMicrofrontends.Core;

/// <summary>
/// Base implementation of the IMicrofrontendModule interface.
/// </summary>
public abstract class MicrofrontendModuleBase : IMicrofrontendModule
{
    private readonly List<(string Path, string Title)> _routes = new();
    private bool _isInitialized;

    /// <summary>
    /// Gets the unique identifier for this microfrontend module.
    /// </summary>
    public abstract string ModuleId { get; }
    
    /// <summary>
    /// Gets the display name of the microfrontend module.
    /// </summary>
    public abstract string Name { get; }
    
    /// <summary>
    /// Gets the version of the microfrontend module.
    /// </summary>
    public abstract string Version { get; }
    
    /// <summary>
    /// Gets the technology/framework used by this microfrontend.
    /// </summary>
    public abstract string Technology { get; }
    
    /// <summary>
    /// Gets the routes supported by this microfrontend module.
    /// </summary>
    public IReadOnlyList<(string Path, string Title)> Routes => _routes;
    
    /// <summary>
    /// Gets whether the module has been initialized.
    /// </summary>
    public bool IsInitialized => _isInitialized;
    
    /// <summary>
    /// Adds a route to the microfrontend module.
    /// </summary>
    /// <param name="path">The path of the route.</param>
    /// <param name="title">The title of the route.</param>
    protected void AddRoute(string path, string title)
    {
        _routes.Add((path, title));
    }
    
    /// <summary>
    /// Configure the services needed by this microfrontend module.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    public virtual void ConfigureServices(IServiceCollection services)
    {
        // Default implementation does nothing
    }
    
    /// <summary>
    /// Initialize the microfrontend module.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            await OnInitializeAsync();
            _isInitialized = true;
        }
    }
    
    /// <summary>
    /// Cleans up resources used by the microfrontend module.
    /// </summary>
    /// <returns>A task representing the asynchronous cleanup operation.</returns>
    public async Task CleanupAsync()
    {
        if (_isInitialized)
        {
            await OnCleanupAsync();
            _isInitialized = false;
        }
    }
    
    /// <summary>
    /// Override to perform custom initialization logic.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    protected virtual Task OnInitializeAsync()
    {
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Override to perform custom cleanup logic.
    /// </summary>
    /// <returns>A task representing the asynchronous cleanup operation.</returns>
    protected virtual Task OnCleanupAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates a new instance of the microfrontend module.
    /// </summary>
    protected MicrofrontendModuleBase()
    {
        _isInitialized = false;
    }
} 