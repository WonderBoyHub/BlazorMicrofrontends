using BlazorMicrofrontends.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorMicrofrontends.Integration;

/// <summary>
/// Extension methods for setting up Blazor Microfrontends integration services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds integration services required for Blazor Microfrontends to function.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBlazorMicrofrontendsIntegration(this IServiceCollection services)
    {
        // Register integration services here
        
        return services;
    }

    /// <summary>
    /// Adds JavaScript microfrontend integration services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddJavaScriptMicrofrontendIntegration(this IServiceCollection services)
    {
        // Ensure core services are registered
        services.AddBlazorMicrofrontends();
        
        return services;
    }
    
    /// <summary>
    /// Adds a React microfrontend module to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the module to.</param>
    /// <param name="moduleId">The unique identifier for the module.</param>
    /// <param name="name">The display name of the module.</param>
    /// <param name="version">The version of the module.</param>
    /// <param name="scriptUrl">The URL of the JavaScript script for this module.</param>
    /// <param name="elementId">The ID of the element where the microfrontend will be mounted.</param>
    /// <param name="mountFunction">The name of the JavaScript function to call to mount the component.</param>
    /// <param name="unmountFunction">The name of the JavaScript function to call to unmount the component.</param>
    /// <param name="cssUrl">The URL of the CSS file for this module, if any.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddReactMicrofrontend(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string scriptUrl,
        string elementId,
        string mountFunction = "mount",
        string unmountFunction = "unmount",
        string? cssUrl = null)
    {
        services.AddSingleton<IMicrofrontendModule>(sp =>
        {
            var jsRuntime = sp.GetRequiredService<IJSRuntime>();
            return new JsMicrofrontendModule(
                moduleId,
                name,
                version,
                "React",
                jsRuntime,
                scriptUrl,
                elementId,
                mountFunction,
                unmountFunction,
                cssUrl);
        });
        
        return services;
    }
    
    /// <summary>
    /// Adds a Vue microfrontend module to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the module to.</param>
    /// <param name="moduleId">The unique identifier for the module.</param>
    /// <param name="name">The display name of the module.</param>
    /// <param name="version">The version of the module.</param>
    /// <param name="scriptUrl">The URL of the JavaScript script for this module.</param>
    /// <param name="elementId">The ID of the element where the microfrontend will be mounted.</param>
    /// <param name="mountFunction">The name of the JavaScript function to call to mount the component.</param>
    /// <param name="unmountFunction">The name of the JavaScript function to call to unmount the component.</param>
    /// <param name="cssUrl">The URL of the CSS file for this module, if any.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddVueMicrofrontend(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string scriptUrl,
        string elementId,
        string mountFunction = "mount",
        string unmountFunction = "unmount",
        string? cssUrl = null)
    {
        services.AddSingleton<IMicrofrontendModule>(sp =>
        {
            var jsRuntime = sp.GetRequiredService<IJSRuntime>();
            return new JsMicrofrontendModule(
                moduleId,
                name,
                version,
                "Vue",
                jsRuntime,
                scriptUrl,
                elementId,
                mountFunction,
                unmountFunction,
                cssUrl);
        });
        
        return services;
    }
    
    /// <summary>
    /// Adds an Angular microfrontend module to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the module to.</param>
    /// <param name="moduleId">The unique identifier for the module.</param>
    /// <param name="name">The display name of the module.</param>
    /// <param name="version">The version of the module.</param>
    /// <param name="scriptUrl">The URL of the JavaScript script for this module.</param>
    /// <param name="elementId">The ID of the element where the microfrontend will be mounted.</param>
    /// <param name="mountFunction">The name of the JavaScript function to call to mount the component.</param>
    /// <param name="unmountFunction">The name of the JavaScript function to call to unmount the component.</param>
    /// <param name="cssUrl">The URL of the CSS file for this module, if any.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddAngularMicrofrontend(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string scriptUrl,
        string elementId,
        string mountFunction = "mount",
        string unmountFunction = "unmount",
        string? cssUrl = null)
    {
        services.AddSingleton<IMicrofrontendModule>(sp =>
        {
            var jsRuntime = sp.GetRequiredService<IJSRuntime>();
            return new JsMicrofrontendModule(
                moduleId,
                name,
                version,
                "Angular",
                jsRuntime,
                scriptUrl,
                elementId,
                mountFunction,
                unmountFunction,
                cssUrl);
        });
        
        return services;
    }
    
    /// <summary>
    /// Adds a custom JavaScript microfrontend module to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the module to.</param>
    /// <param name="moduleId">The unique identifier for the module.</param>
    /// <param name="name">The display name of the module.</param>
    /// <param name="version">The version of the module.</param>
    /// <param name="technology">The technology/framework used by this microfrontend.</param>
    /// <param name="scriptUrl">The URL of the JavaScript script for this module.</param>
    /// <param name="elementId">The ID of the element where the microfrontend will be mounted.</param>
    /// <param name="mountFunction">The name of the JavaScript function to call to mount the component.</param>
    /// <param name="unmountFunction">The name of the JavaScript function to call to unmount the component.</param>
    /// <param name="cssUrl">The URL of the CSS file for this module, if any.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddJavaScriptMicrofrontend(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string technology,
        string scriptUrl,
        string elementId,
        string mountFunction = "mount",
        string unmountFunction = "unmount",
        string? cssUrl = null)
    {
        services.AddSingleton<IMicrofrontendModule>(sp =>
        {
            var jsRuntime = sp.GetRequiredService<IJSRuntime>();
            return new JsMicrofrontendModule(
                moduleId,
                name,
                version,
                technology,
                jsRuntime,
                scriptUrl,
                elementId,
                mountFunction,
                unmountFunction,
                cssUrl);
        });
        
        return services;
    }
} 