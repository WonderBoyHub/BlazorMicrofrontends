using Microsoft.Maui.Controls;

namespace SampleMauiAppShell;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnOpenMicrofrontendsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//deviceinfo");
    }
} 