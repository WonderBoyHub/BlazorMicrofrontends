using Microsoft.Maui.Controls;

namespace SampleMauiAppShell;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
} 