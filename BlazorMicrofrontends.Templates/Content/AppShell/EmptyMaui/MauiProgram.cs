using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BlazorMicrofrontends.AppShell;
using System.Reflection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

namespace EmptyMauiAppShell;

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
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		// Add configuration from appsettings.json
		var assembly = Assembly.GetExecutingAssembly();
		var appSettingsStream = assembly.GetManifestResourceStream("EmptyMauiAppShell.appsettings.json");
		
		builder.Configuration.AddJsonStream(appSettingsStream);

		// Register Blazor Microfrontends services
		builder.Services.AddMicrofrontends(builder.Configuration);

		return builder.Build();
	}
} 