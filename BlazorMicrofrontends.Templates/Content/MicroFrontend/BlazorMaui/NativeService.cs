using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;

namespace BlazorMauiMicrofrontend;

public class NativeService
{
    public async Task<GeolocationResult> GetCurrentLocationAsync()
    {
        GeolocationResult result = new GeolocationResult();
        
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);
            
            if (location != null)
            {
                result.IsSuccess = true;
                result.Latitude = location.Latitude;
                result.Longitude = location.Longitude;
                result.Accuracy = location.Accuracy;
                result.Timestamp = location.Timestamp;
            }
            else
            {
                result.ErrorMessage = "Unable to retrieve location.";
            }
        }
        catch (FeatureNotSupportedException)
        {
            result.ErrorMessage = "Geolocation not supported on this device.";
        }
        catch (PermissionException)
        {
            result.ErrorMessage = "Location permission not granted.";
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"Error retrieving location: {ex.Message}";
        }
        
        return result;
    }
    
    public DeviceDetails GetDeviceDetails()
    {
        return new DeviceDetails
        {
            Manufacturer = DeviceInfo.Current.Manufacturer,
            Model = DeviceInfo.Current.Model,
            Name = DeviceInfo.Current.Name,
            VersionString = DeviceInfo.Current.VersionString,
            Platform = DeviceInfo.Current.Platform.ToString(),
            Idiom = DeviceInfo.Current.Idiom.ToString(),
            DeviceType = DeviceInfo.Current.DeviceType.ToString(),
            BatteryLevel = Battery.Default.ChargeLevel,
            BatteryState = Battery.Default.State.ToString(),
            PowerSource = Battery.Default.PowerSource.ToString()
        };
    }
}

public class GeolocationResult
{
    public bool IsSuccess { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Accuracy { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? ErrorMessage { get; set; }
}

public class DeviceDetails
{
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string VersionString { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Idiom { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public double BatteryLevel { get; set; }
    public string BatteryState { get; set; } = string.Empty;
    public string PowerSource { get; set; } = string.Empty;
} 