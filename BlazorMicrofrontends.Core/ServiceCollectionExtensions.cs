using Microsoft.Extensions.DependencyInjection;

namespace BlazorMicrofrontends.Core;

/// <summary>
/// Extension methods for setting up Blazor Microfrontends core services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core services required for Blazor Microfrontends to function.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBlazorMicrofrontends(this IServiceCollection services)
    {
        services.AddSingleton<IMicrofrontendLifecycle, MicrofrontendLifecycle>();
        services.AddSingleton<IMicrofrontendPackager, NuGetPackager>();
        
        return services;
    }
} 