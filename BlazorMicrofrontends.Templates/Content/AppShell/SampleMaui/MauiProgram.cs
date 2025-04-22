using Microsoft.Extensions.Logging;
using BlazorMicrofrontends.AppShell;
using BlazorMicrofrontends.AppShell.Configuration;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace SampleMauiAppShell;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Add configuration from appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        var appSettingsStream = assembly.GetManifestResourceStream("SampleMauiAppShell.appsettings.json");
        
        builder.Configuration.AddJsonStream(appSettingsStream);

        // Register converters for use in XAML
        builder.Services.AddSingleton<IntToBoolConverter>();

        // Register Blazor Microfrontends services
        builder.Services.AddMicrofrontends(builder.Configuration);

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
} 