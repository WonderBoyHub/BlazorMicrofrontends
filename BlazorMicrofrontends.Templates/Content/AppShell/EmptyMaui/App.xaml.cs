using Microsoft.Maui.Controls;

namespace EmptyMauiAppShell;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
} 