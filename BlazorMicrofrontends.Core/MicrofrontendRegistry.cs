using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BlazorMicrofrontends.Core;

/// <summary>
/// Registry for managing microfrontend modules.
/// </summary>
public class MicrofrontendRegistry
{
    private readonly ConcurrentDictionary<string, IMicrofrontendModule> _modules = new();
    
    /// <summary>
    /// Gets all registered microfrontend modules.
    /// </summary>
    public IEnumerable<IMicrofrontendModule> Modules => _modules.Values;
    
    /// <summary>
    /// Registers a microfrontend module with the registry.
    /// </summary>
    /// <param name="module">The module to register.</param>
    /// <exception cref="ArgumentException">Thrown when a module with the same ID already exists.</exception>
    public void RegisterModule(IMicrofrontendModule module)
    {
        if (!_modules.TryAdd(module.ModuleId, module))
        {
            throw new ArgumentException($"A module with ID '{module.ModuleId}' is already registered.", nameof(module));
        }
    }
    
    /// <summary>
    /// Gets a microfrontend module by its ID.
    /// </summary>
    /// <param name="moduleId">The ID of the module to retrieve.</param>
    /// <returns>The microfrontend module, or null if no module with the specified ID exists.</returns>
    public IMicrofrontendModule? GetModule(string moduleId)
    {
        return _modules.TryGetValue(moduleId, out var module) ? module : null;
    }
    
    /// <summary>
    /// Removes a microfrontend module from the registry.
    /// </summary>
    /// <param name="moduleId">The ID of the module to remove.</param>
    /// <returns>True if the module was removed; otherwise, false.</returns>
    public bool UnregisterModule(string moduleId)
    {
        return _modules.TryRemove(moduleId, out _);
    }
    
    /// <summary>
    /// Initializes all registered modules.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task InitializeAllModulesAsync()
    {
        var initializationTasks = Modules
            .Where(module => !module.IsInitialized)
            .Select(module => module.InitializeAsync());
        
        await Task.WhenAll(initializationTasks);
    }
    
    /// <summary>
    /// Configures services for all registered modules.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        foreach (var module in Modules)
        {
            module.ConfigureServices(services);
        }
    }
} 