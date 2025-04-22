using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;

namespace BlazorMicrofrontends.CLI.Services.Implementations;

public class MicrofrontendService : IMicrofrontendService
{
    private readonly ITemplateService _templateService;
    private readonly IPackageService _packageService;
    
    public MicrofrontendService(ITemplateService templateService, IPackageService packageService)
    {
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
    }
    
    public async Task<bool> CreateMicrofrontendAsync(string templateName, string name, string outputDir, Dictionary<string, string> options)
    {
        // Use the template service to create the microfrontend
        return await _templateService.CreateFromTemplateAsync(templateName, outputDir, name, options);
    }
    
    public async Task<bool> AddMicrofrontendToAppShellAsync(string appShellDir, string microfrontendPath, Dictionary<string, string> options)
    {
        if (string.IsNullOrEmpty(appShellDir))
            throw new ArgumentNullException(nameof(appShellDir));
            
        if (string.IsNullOrEmpty(microfrontendPath))
            throw new ArgumentNullException(nameof(microfrontendPath));
            
        if (!Directory.Exists(appShellDir))
            throw new DirectoryNotFoundException($"App shell directory not found: {appShellDir}");
            
        if (!Directory.Exists(microfrontendPath) && !File.Exists(microfrontendPath))
            throw new FileNotFoundException($"Microfrontend path not found: {microfrontendPath}");
            
        try
        {
            // Get microfrontend info
            var microfrontendInfo = await GetMicrofrontendInfoAsync(microfrontendPath);
            
            // Find appsettings.json
            var appSettingsPath = FindAppSettingsFile(appShellDir);
            
            if (appSettingsPath == null)
            {
                AnsiConsole.WriteLine("No appsettings.json file found. Creating a new one.");
                appSettingsPath = Path.Combine(appShellDir, "appsettings.json");
                
                // Create a default appsettings.json
                var defaultSettings = new
                {
                    Microfrontends = new
                    {
                        Source = "Local",
                        LocalPath = "Microfrontends",
                        RemoteRegistry = "",
                        Enabled = new[] { microfrontendInfo["Name"] }
                    }
                };
                
                File.WriteAllText(appSettingsPath, JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true }));
                
                // Copy microfrontend to LocalPath if it's a directory
                if (Directory.Exists(microfrontendPath))
                {
                    var mfeLocalPath = Path.Combine(appShellDir, "Microfrontends", microfrontendInfo["Name"]);
                    Directory.CreateDirectory(Path.GetDirectoryName(mfeLocalPath));
                    
                    CopyDirectory(microfrontendPath, mfeLocalPath);
                }
                
                return true;
            }
            
            // Read existing appsettings.json
            var appSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                File.ReadAllText(appSettingsPath),
                new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip });
            
            if (appSettings == null)
            {
                appSettings = new Dictionary<string, JsonElement>();
            }
            
            // Get microfrontends configuration
            var microfrontendOptions = new Dictionary<string, object>();
            List<string> enabledMicrofrontends = new List<string>();
            string localPath = "Microfrontends";
            
            if (appSettings.TryGetValue("Microfrontends", out var mfeSection))
            {
                foreach (var prop in mfeSection.EnumerateObject())
                {
                    if (prop.Name == "Enabled" && prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in prop.Value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String)
                            {
                                enabledMicrofrontends.Add(item.GetString());
                            }
                        }
                    }
                    else if (prop.Name == "LocalPath" && prop.Value.ValueKind == JsonValueKind.String)
                    {
                        localPath = prop.Value.GetString();
                    }
                    else
                    {
                        microfrontendOptions[prop.Name] = prop.Value.ValueKind == JsonValueKind.String
                            ? prop.Value.GetString()
                            : prop.Value.ToString();
                    }
                }
            }
            
            // Add the new microfrontend to enabled list if not already there
            if (!enabledMicrofrontends.Contains(microfrontendInfo["Name"]))
            {
                enabledMicrofrontends.Add(microfrontendInfo["Name"]);
            }
            
            microfrontendOptions["Enabled"] = enabledMicrofrontends;
            
            // Create updated appsettings object
            var updatedSettings = new Dictionary<string, object>(appSettings.Count);
            
            foreach (var section in appSettings)
            {
                if (section.Key != "Microfrontends")
                {
                    updatedSettings[section.Key] = section.Value;
                }
            }
            
            updatedSettings["Microfrontends"] = microfrontendOptions;
            
            // Write updated appsettings.json
            File.WriteAllText(appSettingsPath, JsonSerializer.Serialize(updatedSettings, new JsonSerializerOptions { WriteIndented = true }));
            
            // Copy microfrontend to LocalPath if it's a directory
            if (Directory.Exists(microfrontendPath))
            {
                var mfeLocalPath = Path.Combine(appShellDir, localPath, microfrontendInfo["Name"]);
                Directory.CreateDirectory(Path.GetDirectoryName(mfeLocalPath));
                
                CopyDirectory(microfrontendPath, mfeLocalPath);
            }
            
            AnsiConsole.WriteLine($"Added microfrontend {microfrontendInfo["Name"]} to app shell");
            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return false;
        }
    }
    
    public async Task<bool> RemoveMicrofrontendFromAppShellAsync(string appShellDir, string microfrontendId)
    {
        if (string.IsNullOrEmpty(appShellDir))
            throw new ArgumentNullException(nameof(appShellDir));
            
        if (string.IsNullOrEmpty(microfrontendId))
            throw new ArgumentNullException(nameof(microfrontendId));
            
        if (!Directory.Exists(appShellDir))
            throw new DirectoryNotFoundException($"App shell directory not found: {appShellDir}");
            
        try
        {
            // Find appsettings.json
            var appSettingsPath = FindAppSettingsFile(appShellDir);
            
            if (appSettingsPath == null)
            {
                AnsiConsole.WriteLine("No appsettings.json file found. Nothing to remove.");
                return false;
            }
            
            // Read existing appsettings.json
            var appSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                File.ReadAllText(appSettingsPath),
                new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip });
            
            if (appSettings == null || !appSettings.TryGetValue("Microfrontends", out var mfeSection))
            {
                AnsiConsole.WriteLine("No microfrontends configuration found. Nothing to remove.");
                return false;
            }
            
            // Get microfrontends configuration
            var microfrontendOptions = new Dictionary<string, object>();
            List<string> enabledMicrofrontends = new List<string>();
            string localPath = "Microfrontends";
            
            foreach (var prop in mfeSection.EnumerateObject())
            {
                if (prop.Name == "Enabled" && prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            string mfeId = item.GetString();
                            if (mfeId != microfrontendId)
                            {
                                enabledMicrofrontends.Add(mfeId);
                            }
                        }
                    }
                }
                else if (prop.Name == "LocalPath" && prop.Value.ValueKind == JsonValueKind.String)
                {
                    localPath = prop.Value.GetString();
                }
                else
                {
                    microfrontendOptions[prop.Name] = prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString()
                        : prop.Value.ToString();
                }
            }
            
            microfrontendOptions["Enabled"] = enabledMicrofrontends;
            
            // Create updated appsettings object
            var updatedSettings = new Dictionary<string, object>(appSettings.Count);
            
            foreach (var section in appSettings)
            {
                if (section.Key != "Microfrontends")
                {
                    updatedSettings[section.Key] = section.Value;
                }
            }
            
            updatedSettings["Microfrontends"] = microfrontendOptions;
            
            // Write updated appsettings.json
            File.WriteAllText(appSettingsPath, JsonSerializer.Serialize(updatedSettings, new JsonSerializerOptions { WriteIndented = true }));
            
            // Check if there's a local copy to remove
            var mfeLocalPath = Path.Combine(appShellDir, localPath, microfrontendId);
            if (Directory.Exists(mfeLocalPath))
            {
                Directory.Delete(mfeLocalPath, true);
            }
            
            AnsiConsole.WriteLine($"Removed microfrontend {microfrontendId} from app shell");
            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return false;
        }
    }
    
    public async Task<Dictionary<string, string>> GetMicrofrontendInfoAsync(string microfrontendPath)
    {
        if (string.IsNullOrEmpty(microfrontendPath))
            throw new ArgumentNullException(nameof(microfrontendPath));
            
        if (!Directory.Exists(microfrontendPath) && !File.Exists(microfrontendPath))
            throw new FileNotFoundException($"Microfrontend path not found: {microfrontendPath}");
        
        var info = new Dictionary<string, string>();
        
        // Determine if it's a package file or directory
        if (File.Exists(microfrontendPath))
        {
            // For package files, extract basic info from filename
            var fileName = Path.GetFileNameWithoutExtension(microfrontendPath);
            var match = Regex.Match(fileName, @"^(.+?)\.?(\d+\.\d+\.\d+.*)?$");
            
            if (match.Success)
            {
                info["Name"] = match.Groups[1].Value;
                
                if (match.Groups.Count > 2 && !string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    info["Version"] = match.Groups[2].Value;
                }
                else
                {
                    info["Version"] = "1.0.0";
                }
            }
            else
            {
                info["Name"] = fileName;
                info["Version"] = "1.0.0";
            }
            
            string extension = Path.GetExtension(microfrontendPath).ToLowerInvariant();
            
            switch (extension)
            {
                case ".nupkg":
                    info["Technology"] = "Blazor";
                    break;
                case ".tgz":
                    info["Technology"] = "React";
                    break;
                case ".whl":
                    info["Technology"] = "Python";
                    break;
                default:
                    info["Technology"] = "Unknown";
                    break;
            }
            
            info["Route"] = $"/{info["Name"]}";
            info["Enabled"] = "true";
        }
        else
        {
            // For directories, look for microfrontend.json
            var mfeJsonPath = Path.Combine(microfrontendPath, "microfrontend.json");
            
            if (File.Exists(mfeJsonPath))
            {
                try
                {
                    var mfeJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                        File.ReadAllText(mfeJsonPath),
                        new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip });
                    
                    if (mfeJson != null)
                    {
                        foreach (var prop in mfeJson)
                        {
                            if (prop.Value.ValueKind == JsonValueKind.String)
                            {
                                info[prop.Key] = prop.Value.GetString();
                            }
                            else if (prop.Value.ValueKind == JsonValueKind.Array && prop.Key == "routes" && prop.Value.GetArrayLength() > 0)
                            {
                                var firstRoute = prop.Value[0];
                                if (firstRoute.TryGetProperty("path", out var path) && path.ValueKind == JsonValueKind.String)
                                {
                                    info["Route"] = path.GetString();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteLine($"Error reading microfrontend.json: {ex.Message}");
                }
            }
            
            // Set defaults for any missing values
            if (!info.ContainsKey("Name"))
            {
                info["Name"] = Path.GetFileName(microfrontendPath);
            }
            
            if (!info.ContainsKey("Version"))
            {
                info["Version"] = "1.0.0";
            }
            
            if (!info.ContainsKey("Technology"))
            {
                info["Technology"] = DetermineTechnology(microfrontendPath);
            }
            
            if (!info.ContainsKey("Route"))
            {
                info["Route"] = $"/{info["Name"]}";
            }
            
            info["Enabled"] = "true";
        }
        
        return info;
    }
    
    private string DetermineTechnology(string path)
    {
        // Check for presence of files that indicate technology type
        if (Directory.GetFiles(path, "*.csproj").Any())
        {
            var csprojFile = Directory.GetFiles(path, "*.csproj").First();
            var content = File.ReadAllText(csprojFile);
            
            if (content.Contains("Maui") || content.Contains("maui"))
                return "BlazorMaui";
                
            return "Blazor";
        }
        
        if (File.Exists(Path.Combine(path, "package.json")))
        {
            var packageJson = File.ReadAllText(Path.Combine(path, "package.json"));
            
            if (packageJson.Contains("react"))
                return "React";
                
            return "JavaScript";
        }
        
        if (File.Exists(Path.Combine(path, "requirements.txt")) || 
            Directory.GetFiles(path, "*.py").Any())
        {
            return "Python";
        }
        
        // Default to Blazor if can't determine
        return "Blazor";
    }
    
    public async Task<bool> SetMicrofrontendEnabledAsync(string appShellDir, string microfrontendId, bool enabled)
    {
        if (string.IsNullOrEmpty(appShellDir))
            throw new ArgumentNullException(nameof(appShellDir));
            
        if (string.IsNullOrEmpty(microfrontendId))
            throw new ArgumentNullException(nameof(microfrontendId));
            
        if (!Directory.Exists(appShellDir))
            throw new DirectoryNotFoundException($"App shell directory not found: {appShellDir}");
            
        try
        {
            // Find appsettings.json
            var appSettingsPath = FindAppSettingsFile(appShellDir);
            
            if (appSettingsPath == null)
            {
                AnsiConsole.WriteLine("No appsettings.json file found. Cannot update microfrontend state.");
                return false;
            }
            
            // Read existing appsettings.json
            var appSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                File.ReadAllText(appSettingsPath),
                new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip });
            
            if (appSettings == null || !appSettings.TryGetValue("Microfrontends", out var mfeSection))
            {
                AnsiConsole.WriteLine("No microfrontends configuration found. Cannot update microfrontend state.");
                return false;
            }
            
            // Get microfrontends configuration
            var microfrontendOptions = new Dictionary<string, object>();
            List<string> enabledMicrofrontends = new List<string>();
            
            foreach (var prop in mfeSection.EnumerateObject())
            {
                if (prop.Name == "Enabled" && prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            string mfeId = item.GetString();
                            if (mfeId != microfrontendId) // Don't add the target microfrontend yet
                            {
                                enabledMicrofrontends.Add(mfeId);
                            }
                        }
                    }
                }
                else
                {
                    microfrontendOptions[prop.Name] = prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString()
                        : prop.Value.ToString();
                }
            }
            
            // Add the microfrontend to enabled list if it should be enabled
            if (enabled && !enabledMicrofrontends.Contains(microfrontendId))
            {
                enabledMicrofrontends.Add(microfrontendId);
            }
            
            microfrontendOptions["Enabled"] = enabledMicrofrontends;
            
            // Create updated appsettings object
            var updatedSettings = new Dictionary<string, object>(appSettings.Count);
            
            foreach (var section in appSettings)
            {
                if (section.Key != "Microfrontends")
                {
                    updatedSettings[section.Key] = section.Value;
                }
            }
            
            updatedSettings["Microfrontends"] = microfrontendOptions;
            
            // Write updated appsettings.json
            File.WriteAllText(appSettingsPath, JsonSerializer.Serialize(updatedSettings, new JsonSerializerOptions { WriteIndented = true }));
            
            AnsiConsole.WriteLine($"Set microfrontend {microfrontendId} enabled state to {enabled}");
            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return false;
        }
    }
    
    public async Task<bool> PublishMicrofrontendAsync(string microfrontendDir, Dictionary<string, string> options)
    {
        if (string.IsNullOrEmpty(microfrontendDir))
            throw new ArgumentNullException(nameof(microfrontendDir));
            
        if (!Directory.Exists(microfrontendDir))
            throw new DirectoryNotFoundException($"Microfrontend directory not found: {microfrontendDir}");
        
        try
        {
            // First package the microfrontend
            string outputDir = options.TryGetValue("OutputDir", out var outDir) ? outDir : "packages";
            var packagePath = await _packageService.PackageMicrofrontendAsync(microfrontendDir, outputDir, options);
            
            // Check if we need to publish to a feed
            if (options.TryGetValue("Repository", out var repo) && !string.IsNullOrEmpty(repo))
            {
                // Create publish options
                var publishOptions = new Dictionary<string, string>
                {
                    { "feed", repo }
                };
                
                if (options.TryGetValue("ApiKey", out var apiKey) && !string.IsNullOrEmpty(apiKey))
                {
                    publishOptions["apiKey"] = apiKey;
                }
                
                // Publish the package
                return await _packageService.PublishPackageAsync(packagePath, publishOptions);
            }
            
            // Just packaging succeeded
            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return false;
        }
    }
    
    private string FindAppSettingsFile(string appShellDir)
    {
        // Look for appsettings.json in common locations
        var possiblePaths = new[]
        {
            Path.Combine(appShellDir, "appsettings.json"),
            Path.Combine(appShellDir, "wwwroot", "appsettings.json"),
            Path.Combine(appShellDir, "config", "appsettings.json")
        };
        
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
                return path;
        }
        
        // If not found, search recursively but limited to avoid deep traversal
        return FindFileRecursive(appShellDir, "appsettings.json", 3);
    }
    
    private string FindFileRecursive(string directory, string fileName, int maxDepth)
    {
        if (maxDepth <= 0) return null;
        
        foreach (var file in Directory.GetFiles(directory))
        {
            if (Path.GetFileName(file).Equals(fileName, StringComparison.OrdinalIgnoreCase))
                return file;
        }
        
        foreach (var subDir in Directory.GetDirectories(directory))
        {
            var result = FindFileRecursive(subDir, fileName, maxDepth - 1);
            if (result != null)
                return result;
        }
        
        return null;
    }
    
    private void CopyDirectory(string sourceDirName, string destDirName)
    {
        // Create the destination directory
        Directory.CreateDirectory(destDirName);

        // Copy files
        foreach (string file in Directory.GetFiles(sourceDirName))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destDirName, fileName);
            File.Copy(file, destFile, true);
        }

        // Copy subdirectories recursively
        foreach (string subdir in Directory.GetDirectories(sourceDirName))
        {
            string dirName = Path.GetFileName(subdir);
            
            // Skip .git directories
            if (dirName.Equals(".git", StringComparison.OrdinalIgnoreCase))
                continue;
                
            string destSubDir = Path.Combine(destDirName, dirName);
            CopyDirectory(subdir, destSubDir);
        }
    }
} 