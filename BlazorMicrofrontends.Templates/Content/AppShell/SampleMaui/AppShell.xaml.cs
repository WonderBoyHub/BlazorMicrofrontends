using Microsoft.Maui.Controls;

namespace SampleMauiAppShell;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(BlazorPage), typeof(BlazorPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
    }
} 