namespace BlazorMicrofrontends.Core;

/// <summary>
/// Implementation of the microfrontend lifecycle manager.
/// </summary>
public class MicrofrontendLifecycle : IMicrofrontendLifecycle
{
    private readonly MicrofrontendRegistry _registry;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MicrofrontendLifecycle"/> class.
    /// </summary>
    /// <param name="registry">The microfrontend registry.</param>
    public MicrofrontendLifecycle(MicrofrontendRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }
    
    /// <inheritdoc />
    public async Task InitializeAllModulesAsync()
    {
        await _registry.InitializeAllModulesAsync();
    }
    
    /// <inheritdoc />
    public async Task InitializeModuleAsync(string moduleId)
    {
        var module = _registry.GetModule(moduleId);
        if (module == null)
        {
            throw new ArgumentException($"No module with ID '{moduleId}' is registered.", nameof(moduleId));
        }
        
        if (!module.IsInitialized)
        {
            await module.InitializeAsync();
        }
    }
    
    /// <inheritdoc />
    public IEnumerable<IMicrofrontendModule> GetAllModules()
    {
        return _registry.Modules;
    }
    
    /// <inheritdoc />
    public IMicrofrontendModule? GetModule(string moduleId)
    {
        return _registry.GetModule(moduleId);
    }
} 