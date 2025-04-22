using Microsoft.Extensions.DependencyInjection;

namespace BlazorMauiMicrofrontend;

public static class MicrofrontendServiceCollectionExtensions
{
    public static IServiceCollection AddMauiMicrofrontendServices(this IServiceCollection services)
    {
        // Register microfrontend services
        services.AddScoped<NativeService>();
        
        return services;
    }
} 