using BlazorMicrofrontends.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorMicrofrontends.Host;

/// <summary>
/// Extension methods for setting up Blazor Microfrontends host services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds host services required for Blazor Microfrontends to function.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBlazorMicrofrontendsHost(this IServiceCollection services)
    {
        // First ensure the core services are registered
        services.AddBlazorMicrofrontends();
        
        // Register host-specific services here
        services.AddSingleton<IMicrofrontendLifecycle, MicrofrontendLifecycle>();
        services.AddSingleton<MicrofrontendRegistry>();
        
        return services;
    }
    
    /// <summary>
    /// Adds a Blazor component as a microfrontend module.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Blazor component to add.</typeparam>
    /// <param name="services">The service collection to add the module to.</param>
    /// <param name="moduleId">The unique identifier for the module.</param>
    /// <param name="name">The display name of the module.</param>
    /// <param name="version">The version of the module.</param>
    /// <param name="isWebAssembly">Whether this module represents a Blazor WebAssembly component.</param>
    /// <param name="renderMode">The render mode for the Blazor component.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddBlazorMicrofrontend<TComponent>(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        bool isWebAssembly = false,
        IComponentRenderMode? renderMode = null) where TComponent : class, IComponent
    {
        // Create the module
        var module = new BlazorMicrofrontendModule(
            moduleId,
            name,
            version,
            typeof(TComponent),
            isWebAssembly,
            renderMode);
        
        // Register the module with the registry
        services.AddSingleton<IMicrofrontendModule>(module);
        
        // Register the component type
        services.AddTransient<TComponent>();
        
        return services;
    }
} 