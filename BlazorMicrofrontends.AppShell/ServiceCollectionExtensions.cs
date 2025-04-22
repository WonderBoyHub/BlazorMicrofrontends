using BlazorMicrofrontends.Core;
using BlazorMicrofrontends.AppShell.Services;
using BlazorMicrofrontends.Host;
using BlazorMicrofrontends.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlazorMicrofrontends.AppShell;

/// <summary>
/// Extension methods for setting up Blazor Microfrontend AppShell services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services required for the BlazorMicrofrontends app shell to function.
    /// This includes components for routing and loading microfrontends.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureAction">Optional action to configure settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBlazorMicrofrontendsAppShell(this IServiceCollection services, Action<AppShellOptions>? configureAction = null)
    {
        // Configure options
        var options = new AppShellOptions();
        configureAction?.Invoke(options);
        services.AddSingleton(options);

        // Add all required services in the correct order
        services.AddBlazorMicrofrontends();
        services.AddBlazorMicrofrontendsHost();
        services.AddBlazorMicrofrontendsIntegration();
        
        // Register AppShell specific services - the MicrofrontendRegistry implementation
        // from AppShell that delegates to the Core MicrofrontendRegistry
        services.AddSingleton<IMicrofrontendRegistry, Services.MicrofrontendRegistry>();

        return services;
    }

    /// <summary>
    /// Configures Blazor Microfrontends from configuration in appsettings.json.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The configuration containing the microfrontend settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection ConfigureBlazorMicrofrontendsFromConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var microfrontendSection = configuration.GetSection("Microfrontends");
        if (!microfrontendSection.Exists())
        {
            return services;
        }

        var microfrontends = microfrontendSection.Get<List<MicrofrontendConfiguration>>();
        if (microfrontends == null || !microfrontends.Any())
        {
            return services;
        }

        // Use a provider to resolve services
        var serviceProvider = services.BuildServiceProvider();
        var registry = serviceProvider.GetRequiredService<IMicrofrontendRegistry>();
        
        // Load microfrontends from configuration
        foreach (var config in microfrontends)
        {
            if (string.IsNullOrEmpty(config.Id) || string.IsNullOrEmpty(config.Technology))
            {
                continue;
            }

            // Register microfrontends based on technology
            switch (config.Technology.ToLowerInvariant())
            {
                case "blazor":
                    // For Blazor, we'd need to look up the component type, which might require more context
                    // This is left as a placeholder and would require reflection or a more complex registration
                    break;
                case "react":
                    RegisterJavaScriptModule(registry, config, "React");
                    break;
                case "vue":
                    RegisterJavaScriptModule(registry, config, "Vue");
                    break;
                case "angular":
                    RegisterJavaScriptModule(registry, config, "Angular");
                    break;
                default:
                    // Custom technology
                    RegisterJavaScriptModule(registry, config, config.Technology);
                    break;
            }
        }

        return services;
    }

    private static void RegisterJavaScriptModule(IMicrofrontendRegistry registry, MicrofrontendConfiguration config, string technology)
    {
        // Create a JavaScript module directly via constructor and register it
        // Note: This approach depends on the JsMicrofrontendModule constructor and would require
        // a real IServiceProvider/IJSRuntime for a real implementation
    }
}

/// <summary>
/// Options for configuring the AppShell.
/// </summary>
public class AppShellOptions
{
    /// <summary>
    /// Gets or sets whether to automatically discover and register microfrontends.
    /// </summary>
    public bool AutoDiscover { get; set; } = true;

    /// <summary>
    /// Gets or sets directories to search for microfrontends.
    /// </summary>
    public List<string> MicrofrontendDirectories { get; set; } = new List<string>();
}

/// <summary>
/// Configuration for a microfrontend from appsettings.json.
/// </summary>
public class MicrofrontendConfiguration
{
    /// <summary>
    /// Gets or sets the unique identifier for the microfrontend.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the microfrontend.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the microfrontend.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the technology used for the microfrontend.
    /// </summary>
    public string Technology { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entry point for the microfrontend.
    /// For JavaScript modules, this is the script URL.
    /// </summary>
    public string EntryPoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CSS URL for JavaScript modules.
    /// </summary>
    public string CssUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the element ID for mounting JavaScript components.
    /// </summary>
    public string ElementId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the routes for the microfrontend.
    /// </summary>
    public List<RouteConfiguration> Routes { get; set; } = new List<RouteConfiguration>();
}

/// <summary>
/// Configuration for a microfrontend route from appsettings.json.
/// </summary>
public class RouteConfiguration
{
    /// <summary>
    /// Gets or sets the path for the route.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title for the route.
    /// </summary>
    public string Title { get; set; } = string.Empty;
} 