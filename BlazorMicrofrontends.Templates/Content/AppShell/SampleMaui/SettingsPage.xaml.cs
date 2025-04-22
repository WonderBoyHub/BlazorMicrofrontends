using Microsoft.Maui.Controls;
using System.IO;
using System.Text.Json;

namespace SampleMauiAppShell;

public partial class SettingsPage : ContentPage
{
    private readonly string _configFilePath;

    public SettingsPage()
    {
        InitializeComponent();
        
        _configFilePath = Path.Combine(FileSystem.AppDataDirectory, "appsettings.json");
        LoadSettings();
    }

    private void OnSourceTypeChanged(object sender, EventArgs e)
    {
        // This is handled by XAML binding
    }

    private async void OnSaveSettingsClicked(object sender, EventArgs e)
    {
        var settings = new AppSettings
        {
            Microfrontends = new MicrofrontendSettings
            {
                Source = SourceTypePicker.SelectedIndex == 0 ? "Local" : "Remote",
                LocalPath = LocalPathEntry.Text?.Trim() ?? "Microfrontends",
                RemoteRegistry = RemoteUrlEntry.Text?.Trim() ?? string.Empty
            }
        };

        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configFilePath, json);

        await DisplayAlert("Settings", "Settings saved successfully", "OK");
    }

    private void LoadSettings()
    {
        if (File.Exists(_configFilePath))
        {
            try
            {
                var json = File.ReadAllText(_configFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);

                if (settings != null)
                {
                    SourceTypePicker.SelectedIndex = settings.Microfrontends.Source == "Local" ? 0 : 1;
                    LocalPathEntry.Text = settings.Microfrontends.LocalPath;
                    RemoteUrlEntry.Text = settings.Microfrontends.RemoteRegistry;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }
        }
        else
        {
            // Set defaults
            SourceTypePicker.SelectedIndex = 0;
            LocalPathEntry.Text = "Microfrontends";
            RemoteUrlEntry.Text = string.Empty;
        }
    }
}

public class AppSettings
{
    public MicrofrontendSettings Microfrontends { get; set; } = new MicrofrontendSettings();
}

public class MicrofrontendSettings
{
    public string Source { get; set; } = "Local";
    public string LocalPath { get; set; } = "Microfrontends";
    public string RemoteRegistry { get; set; } = string.Empty;
} 