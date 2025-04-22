# Blazor Microfrontends MAUI Integration Guide

<p align="center">
  <img src="assets/maui-integration-header.svg" alt="Blazor MAUI Integration" width="800" />
</p>

This guide explains how to integrate the Blazor Microfrontends SDK with .NET MAUI applications to create cross-platform microfrontend-based applications for Android, iOS, Windows, and macOS.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Setting Up a MAUI App Shell](#setting-up-a-maui-app-shell)
  - [Creating a New MAUI App Shell](#creating-a-new-maui-app-shell)
  - [Installing Required Packages](#installing-required-packages)
  - [Configuring MAUI Services](#configuring-maui-services)
  - [Setting Up appsettings.json](#setting-up-appsettingsjson)
- [Creating MAUI-Specific Microfrontends](#creating-maui-specific-microfrontends)
  - [Creating a New MAUI Microfrontend](#creating-a-new-maui-microfrontend)
  - [Implementing MAUI Features](#implementing-maui-features)
- [Accessing Native Platform Features](#accessing-native-platform-features)
  - [Device Information](#device-information)
  - [Geolocation](#geolocation)
  - [Camera and Media](#camera-and-media)
  - [File System Access](#file-system-access)
- [Best Practices for MAUI Microfrontends](#best-practices-for-maui-microfrontends)
  - [Cross-Platform Considerations](#cross-platform-considerations)
  - [Performance Optimization](#performance-optimization)
  - [Handling Device-Specific Features](#handling-device-specific-features)
  - [Offline Support](#offline-support)
- [Example Projects](#example-projects)
  - [Device Information Microfrontend](#device-information-microfrontend)
  - [Map and Location Microfrontend](#map-and-location-microfrontend)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Before you begin, ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET MAUI workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation)
- [Blazor Microfrontends CLI](getting-started.md#installation)

You'll also need platform-specific development tools:
- For Android: Android SDK
- For iOS/macOS: Xcode (macOS only)
- For Windows: Windows 10/11 with Windows App SDK

## Setting Up a MAUI App Shell

### Creating a New MAUI App Shell

You can create a new MAUI app shell using the Blazor Microfrontends CLI:

```bash
bmf new app-shell -t EmptyMaui -n MyMauiMicrofrontends
```

This will create a new MAUI application configured to host microfrontends. 

Alternatively, you can create a sample MAUI app shell with pre-configured examples:

```bash
bmf new app-shell -t SampleMaui -n MyMauiMicrofrontends
```

### Installing Required Packages

If you're integrating with an existing MAUI application, you'll need to install the following NuGet packages:

```bash
dotnet add package BlazorMicrofrontends.AppShell
dotnet add package Microsoft.AspNetCore.Components.WebView.Maui
```

### Configuring MAUI Services

In your `MauiProgram.cs` file, register the required services:

```csharp
using BlazorMicrofrontends.AppShell;
using BlazorMicrofrontends.AppShell.Configuration;
using System.Reflection;
using Microsoft.Extensions.Configuration;

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
        var appSettingsStream = assembly.GetManifestResourceStream(
            "MyMauiMicrofrontends.appsettings.json");
        
        builder.Configuration.AddJsonStream(appSettingsStream);

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
```

### Setting Up appsettings.json

Create an `appsettings.json` file in your project root and set it to be embedded as a resource:

```json
{
  "Microfrontends": {
    "Source": "Local",
    "LocalPath": "Microfrontends",
    "RemoteRegistry": "",
    "Enabled": []
  }
}
```

In your `.csproj` file, ensure the file is embedded as a resource:

```xml
<ItemGroup>
  <EmbeddedResource Include="appsettings.json" />
</ItemGroup>
```

## Creating MAUI-Specific Microfrontends

### Creating a New MAUI Microfrontend

Create a new MAUI-specific microfrontend using the CLI:

```bash
bmf new microfrontend -t BlazorMaui -n DeviceInfo
```

This will create a new microfrontend project optimized for MAUI integration.

### Implementing MAUI Features

The BlazorMaui template includes a main component that demonstrates accessing MAUI-specific features:

```csharp
@page "/{**path}"
@using BlazorMicrofrontends.Microfrontend.Interfaces
@using Microsoft.Maui.Devices
@inject IMicrofrontendService MicrofrontendService

<div class="container my-4">
    <h2>@MicrofrontendService.GetName()</h2>
    
    <div class="card mb-4">
        <div class="card-header">
            <h3 class="card-title">Device Information</h3>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <label class="fw-bold">Manufacturer:</label>
                <span>@DeviceInfo.Current.Manufacturer</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Model:</label>
                <span>@DeviceInfo.Current.Model</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Platform:</label>
                <span>@DeviceInfo.Platform</span>
            </div>
        </div>
    </div>
</div>
```

## Accessing Native Platform Features

### Device Information

Access device information using the MAUI `DeviceInfo` class:

```csharp
@using Microsoft.Maui.Devices

<div class="device-info">
    <p>Manufacturer: @DeviceInfo.Current.Manufacturer</p>
    <p>Model: @DeviceInfo.Current.Model</p>
    <p>Name: @DeviceInfo.Current.Name</p>
    <p>Version: @DeviceInfo.Current.VersionString</p>
    <p>Platform: @DeviceInfo.Current.Platform</p>
    <p>Idiom: @DeviceInfo.Current.Idiom</p>
</div>
```

### Geolocation

Access location services using the MAUI `Geolocation` class:

```csharp
@using Microsoft.Maui.Devices.Sensors
@inject IJSRuntime JSRuntime

<button class="btn btn-primary" @onclick="GetLocationAsync">Get Location</button>

<div class="location-info @(location != null ? "d-block" : "d-none")">
    <p>Latitude: @location?.Latitude</p>
    <p>Longitude: @location?.Longitude</p>
    <p>Accuracy: @location?.Accuracy meters</p>
</div>

@code {
    private Location location;

    private async Task GetLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            location = await Geolocation.GetLocationAsync(request);
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error getting location: {ex.Message}");
        }
    }
}
```

### Camera and Media

Access the camera using the MAUI `MediaPicker` class:

```csharp
@using Microsoft.Maui.Storage

<button class="btn btn-primary" @onclick="TakePhotoAsync">Take Photo</button>

<div class="photo-container @(photoPath != null ? "d-block" : "d-none")">
    <img src="@photoPath" style="max-width: 100%;" />
</div>

@code {
    private string photoPath;

    private async Task TakePhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            
            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                var localPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                
                using (var fileStream = File.Create(localPath))
                {
                    await stream.CopyToAsync(fileStream);
                }
                
                photoPath = localPath;
            }
        }
        catch (Exception ex)
        {
            // Handle error
        }
    }
}
```

### File System Access

Access the file system using MAUI's `FileSystem` class:

```csharp
@using Microsoft.Maui.Storage

<button class="btn btn-primary" @onclick="SaveFileAsync">Save File</button>
<button class="btn btn-secondary" @onclick="LoadFileAsync">Load File</button>

<textarea @bind="fileContent" rows="10" class="form-control mt-3"></textarea>

@code {
    private string fileContent = "Sample content";
    private readonly string fileName = "sample.txt";

    private async Task SaveFileAsync()
    {
        try
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            File.WriteAllText(filePath, fileContent);
            await JSRuntime.InvokeVoidAsync("alert", $"File saved to {filePath}");
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error saving file: {ex.Message}");
        }
    }

    private async Task LoadFileAsync()
    {
        try
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            
            if (File.Exists(filePath))
            {
                fileContent = File.ReadAllText(filePath);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "File does not exist");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error loading file: {ex.Message}");
        }
    }
}
```

## Best Practices for MAUI Microfrontends

### Cross-Platform Considerations

When developing MAUI microfrontends, consider the following cross-platform best practices:

1. **Use Conditional Compilation**: Use `#if` directives for platform-specific code:

```csharp
private async Task TakePhotoAsync()
{
#if ANDROID
    // Android-specific implementation
#elif IOS
    // iOS-specific implementation
#else
    // Default implementation
#endif
}
```

2. **Implement Platform-Specific Services**: Use dependency injection to provide platform-specific implementations:

```csharp
public interface INativeService
{
    Task<string> GetPlatformSpecificDataAsync();
}

#if ANDROID
[Register("MyApp.Droid.AndroidNativeService")]
public class AndroidNativeService : INativeService
{
    // Android implementation
}
#elif IOS
[Register("MyApp.iOS.iOSNativeService")]
public class iOSNativeService : INativeService
{
    // iOS implementation
}
#endif
```

3. **Test on All Target Platforms**: Ensure your microfrontends work correctly on all platforms you intend to support.

### Performance Optimization

1. **Minimize JavaScript Interop**: Excessive JS interop can impact performance.
2. **Use Efficient Rendering**: Implement virtualization for large lists.
3. **Optimize Images**: Use appropriate image formats and sizes for mobile devices.
4. **Lazy Load Microfrontends**: Load microfrontends only when needed.

### Handling Device-Specific Features

1. **Graceful Degradation**: Provide fallbacks when specific features aren't available.
2. **Feature Detection**: Check if a feature is available before using it:

```csharp
if (MediaPicker.Default.IsCaptureSupported)
{
    // Use camera
}
else
{
    // Show alternative UI
}
```

### Offline Support

1. **Local Storage**: Use `FileSystem.AppDataDirectory` to store data locally.
2. **Offline-First Design**: Design microfrontends to work offline first.
3. **Synchronization**: Implement a synchronization mechanism when online.

## Example Projects

### Device Information Microfrontend

A complete DeviceInfo microfrontend example:

```csharp
@page "/{**path}"
@using BlazorMicrofrontends.Microfrontend.Interfaces
@using Microsoft.Maui.Devices
@using Microsoft.Maui.Devices.Sensors
@inject IMicrofrontendService MicrofrontendService

<div class="container my-4">
    <h2>Device Information</h2>
    
    <div class="card mb-4">
        <div class="card-header">
            <h3 class="card-title">Basic Info</h3>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <label class="fw-bold">Manufacturer:</label>
                <span>@DeviceInfo.Current.Manufacturer</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Model:</label>
                <span>@DeviceInfo.Current.Model</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Platform:</label>
                <span>@DeviceInfo.Platform</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Device Type:</label>
                <span>@DeviceInfo.Current.DeviceType</span>
            </div>
        </div>
    </div>
    
    <div class="card mb-4">
        <div class="card-header">
            <h3 class="card-title">Battery</h3>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <label class="fw-bold">Battery Level:</label>
                <div class="progress">
                    <div class="progress-bar @GetBatteryColorClass()" 
                         role="progressbar" 
                         style="width: @(Battery.Default.ChargeLevel * 100)%"
                         aria-valuenow="@(Battery.Default.ChargeLevel * 100)" 
                         aria-valuemin="0" 
                         aria-valuemax="100">
                        @(Math.Round(Battery.Default.ChargeLevel * 100))%
                    </div>
                </div>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Power Source:</label>
                <span>@Battery.Default.PowerSource</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Battery State:</label>
                <span>@Battery.Default.State</span>
            </div>
        </div>
    </div>
    
    <div class="card mb-4">
        <div class="card-header">
            <h3 class="card-title">Display</h3>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <label class="fw-bold">Screen Density:</label>
                <span>@DeviceDisplay.Current.MainDisplayInfo.Density</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Orientation:</label>
                <span>@DeviceDisplay.Current.MainDisplayInfo.Orientation</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Rotation:</label>
                <span>@DeviceDisplay.Current.MainDisplayInfo.Rotation</span>
            </div>
            <div class="mb-3">
                <label class="fw-bold">Refresh Rate:</label>
                <span>@DeviceDisplay.Current.MainDisplayInfo.RefreshRate Hz</span>
            </div>
        </div>
    </div>
</div>

@code {
    private string GetBatteryColorClass()
    {
        var level = Battery.Default.ChargeLevel;
        
        if (level < 0.2) return "bg-danger";
        if (level < 0.5) return "bg-warning";
        return "bg-success";
    }
}
```

### Map and Location Microfrontend

A map and location microfrontend example using MAUI's geolocation services:

```csharp
@page "/{**path}"
@using BlazorMicrofrontends.Microfrontend.Interfaces
@using Microsoft.Maui.Devices.Sensors
@inject IMicrofrontendService MicrofrontendService
@inject IJSRuntime JSRuntime

<div class="container my-4">
    <h2>Map & Location</h2>
    
    <div class="card mb-4">
        <div class="card-header d-flex justify-content-between align-items-center">
            <h3 class="card-title mb-0">Current Location</h3>
            <button class="btn btn-primary" @onclick="GetLocationAsync">
                @(isLoading ? "Loading..." : "Get Location")
            </button>
        </div>
        <div class="card-body">
            @if (location != null)
            {
                <div class="mb-3">
                    <label class="fw-bold">Latitude:</label>
                    <span>@location.Latitude</span>
                </div>
                <div class="mb-3">
                    <label class="fw-bold">Longitude:</label>
                    <span>@location.Longitude</span>
                </div>
                <div class="mb-3">
                    <label class="fw-bold">Accuracy:</label>
                    <span>@location.Accuracy meters</span>
                </div>
                <div class="mb-3">
                    <label class="fw-bold">Timestamp:</label>
                    <span>@location.Timestamp.LocalDateTime</span>
                </div>
                
                <div id="map" style="height: 300px; width: 100%;"></div>
            }
            else if (errorMessage != null)
            {
                <div class="alert alert-danger">
                    @errorMessage
                </div>
            }
            else if (!isLoading)
            {
                <div class="alert alert-info">
                    Press "Get Location" to see your current position.
                </div>
            }
            else
            {
                <div class="d-flex justify-content-center">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                </div>
            }
        </div>
    </div>
</div>

@code {
    private Location location;
    private string errorMessage;
    private bool isLoading = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && location != null)
        {
            await InitializeMapAsync();
        }
    }
    
    private async Task GetLocationAsync()
    {
        try
        {
            isLoading = true;
            errorMessage = null;
            StateHasChanged();
            
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            location = await Geolocation.GetLocationAsync(request);
            
            if (location != null)
            {
                await InitializeMapAsync();
            }
        }
        catch (FeatureNotSupportedException)
        {
            errorMessage = "Geolocation is not supported on this device.";
        }
        catch (PermissionException)
        {
            errorMessage = "Location permission not granted.";
        }
        catch (Exception ex)
        {
            errorMessage = $"Error getting location: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
    
    private async Task InitializeMapAsync()
    {
        if (location != null)
        {
            await JSRuntime.InvokeVoidAsync("initializeMap", location.Latitude, location.Longitude);
        }
    }
}
```

Add the necessary JavaScript for the map:

```html
<!-- In your index.html -->
<script src="https://unpkg.com/leaflet@1.9.3/dist/leaflet.js"></script>
<link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.3/dist/leaflet.css" />

<script>
    let map;
    
    window.initializeMap = function (lat, lng) {
        if (map) {
            map.remove();
        }
        
        map = L.map('map').setView([lat, lng], 15);
        
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);
        
        L.marker([lat, lng]).addTo(map)
            .bindPopup('Your location')
            .openPopup();
    };
</script>
```

## Troubleshooting

### Common Issues

1. **App Shell doesn't load microfrontends**
   - Check that the microfrontend is enabled in `appsettings.json`
   - Verify the microfrontend is in the correct directory
   - Check for any JavaScript errors in the browser console

2. **Platform-specific features not working**
   - Ensure you have the necessary permissions in your `AndroidManifest.xml` (Android) or `Info.plist` (iOS)
   - Verify that the feature is supported on the target platform
   - Check that you're using the correct MAUI API

3. **Build errors with MAUI**
   - Ensure you have the latest MAUI workload installed
   - Check that all NuGet packages are compatible
   - Make sure you're targeting compatible platforms

For more help, visit our [GitHub repository](https://github.com/blazor-microfrontends/sdk) or join our [Discord community](https://discord.gg/blazor-microfrontends). 