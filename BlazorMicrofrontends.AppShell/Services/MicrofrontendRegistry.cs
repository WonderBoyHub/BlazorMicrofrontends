using System;
using System.Collections.Generic;
using System.Linq;
using BlazorMicrofrontends.Core;

namespace BlazorMicrofrontends.AppShell.Services;

/// <summary>
/// Implementation of <see cref="IMicrofrontendRegistry"/> that delegates to the Core MicrofrontendRegistry.
/// </summary>
public class MicrofrontendRegistry : IMicrofrontendRegistry
{
    private readonly Core.MicrofrontendRegistry _coreRegistry;

    /// <summary>
    /// Creates a new instance of <see cref="MicrofrontendRegistry"/>.
    /// </summary>
    /// <param name="coreRegistry">The core registry to delegate to.</param>
    public MicrofrontendRegistry(Core.MicrofrontendRegistry coreRegistry)
    {
        _coreRegistry = coreRegistry ?? throw new ArgumentNullException(nameof(coreRegistry));
    }

    /// <summary>
    /// Gets all registered microfrontends.
    /// </summary>
    /// <returns>A collection of microfrontend modules.</returns>
    public IEnumerable<IMicrofrontendModule> GetMicrofrontends()
    {
        return _coreRegistry.Modules;
    }

    /// <summary>
    /// Gets a microfrontend module by its ID.
    /// </summary>
    /// <param name="moduleId">The ID of the microfrontend module.</param>
    /// <returns>The microfrontend module if found; otherwise, null.</returns>
    public IMicrofrontendModule? GetMicrofrontend(string moduleId)
    {
        return _coreRegistry.GetModule(moduleId);
    }

    /// <summary>
    /// Registers a microfrontend module.
    /// </summary>
    /// <param name="module">The microfrontend module to register.</param>
    public void RegisterMicrofrontend(IMicrofrontendModule module)
    {
        if (module == null)
            throw new ArgumentNullException(nameof(module));

        if (string.IsNullOrEmpty(module.ModuleId))
            throw new ArgumentException("Microfrontend module must have a non-empty ID", nameof(module));

        _coreRegistry.RegisterModule(module);
    }

    /// <summary>
    /// Unregisters a microfrontend module.
    /// </summary>
    /// <param name="moduleId">The ID of the microfrontend module to unregister.</param>
    public void UnregisterMicrofrontend(string moduleId)
    {
        if (string.IsNullOrEmpty(moduleId))
            throw new ArgumentException("Module ID cannot be null or empty", nameof(moduleId));

        _coreRegistry.UnregisterModule(moduleId);
    }
} 