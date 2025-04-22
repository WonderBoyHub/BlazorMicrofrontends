using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;

namespace BlazorMicrofrontends.CLI.Services.Implementations;

public class AppShellService : IAppShellService
{
    private readonly ITemplateService _templateService;
    private readonly IPackageService _packageService;
    
    public AppShellService(ITemplateService templateService, IPackageService packageService)
    {
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
    }
    
    public async Task<bool> CreateAppShellAsync(string templateName, string name, string outputDir, Dictionary<string, string> options)
    {
        // Use the template service to create the app shell
        return await _templateService.CreateFromTemplateAsync(templateName, outputDir, name, options);
    }
    
    public async Task<bool> ConfigureAppShellAsync(string appShellDir, Dictionary<string, string> options)
    {
        if (string.IsNullOrEmpty(appShellDir))
            throw new ArgumentNullException(nameof(appShellDir));
            
        if (!Directory.Exists(appShellDir))
            throw new DirectoryNotFoundException($"App shell directory not found: {appShellDir}");
        
        try
        {
            // Look for appsettings.json
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
                        RemoteRegistry = ""
                    }
                };
                
                File.WriteAllText(appSettingsPath, JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true }));
            }
            
            // Read existing appsettings.json
            var appSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                File.ReadAllText(appSettingsPath),
                new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip });
            
            if (appSettings == null)
            {
                appSettings = new Dictionary<string, JsonElement>();
            }
            
            // Update microfrontends section
            var microfrontendOptions = new Dictionary<string, object>();
            
            if (appSettings.TryGetValue("Microfrontends", out var mfeSection))
            {
                foreach (var prop in mfeSection.EnumerateObject())
                {
                    microfrontendOptions[prop.Name] = prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString()
                        : prop.Value.ToString();
                }
            }
            
            // Update with provided options
            foreach (var option in options)
            {
                microfrontendOptions[option.Key] = option.Value;
            }
            
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
            
            AnsiConsole.WriteLine($"Updated configuration in {appSettingsPath}");
            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return false;
        }
    }
    
    public async Task<IEnumerable<string>> ListMicrofrontendsAsync(string appShellDir)
    {
        if (string.IsNullOrEmpty(appShellDir))
            throw new ArgumentNullException(nameof(appShellDir));
            
        if (!Directory.Exists(appShellDir))
            throw new DirectoryNotFoundException($"App shell directory not found: {appShellDir}");
        
        var microfrontends = new List<string>();
        
        // Find appsettings.json to determine microfrontend source
        var appSettingsPath = FindAppSettingsFile(appShellDir);
        
        if (appSettingsPath != null)
        {
            var appSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                File.ReadAllText(appSettingsPath),
                new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip });
            
            if (appSettings?.TryGetValue("Microfrontends", out var mfeSection) == true)
            {
                string source = "Local";
                string localPath = "Microfrontends";
                string remoteRegistry = "";
                
                foreach (var prop in mfeSection.EnumerateObject())
                {
                    if (prop.Name == "Source" && prop.Value.ValueKind == JsonValueKind.String)
                        source = prop.Value.GetString();
                    else if (prop.Name == "LocalPath" && prop.Value.ValueKind == JsonValueKind.String)
                        localPath = prop.Value.GetString();
                    else if (prop.Name == "RemoteRegistry" && prop.Value.ValueKind == JsonValueKind.String)
                        remoteRegistry = prop.Value.GetString();
                }
                
                if (source == "Local")
                {
                    // Check local directory for microfrontends
                    var microfrontendsDir = Path.Combine(appShellDir, localPath);
                    
                    if (Directory.Exists(microfrontendsDir))
                    {
                        foreach (var mfeDir in Directory.GetDirectories(microfrontendsDir))
                        {
                            // Check for microfrontend.json to validate
                            var mfeJsonPath = Path.Combine(mfeDir, "microfrontend.json");
                            if (File.Exists(mfeJsonPath))
                            {
                                microfrontends.Add(Path.GetFileName(mfeDir));
                            }
                        }
                    }
                }
                else if (source == "Remote" && !string.IsNullOrEmpty(remoteRegistry))
                {
                    // List remote packages 
                    var packages = await _packageService.ListPackagesAsync(remoteRegistry, new Dictionary<string, string>());
                    
                    foreach (var package in packages)
                    {
                        if (package.Id.Contains("Microfrontend") || package.Tags?.Contains("microfrontend") == true)
                        {
                            microfrontends.Add(package.Id);
                        }
                    }
                }
            }
        }
        
        return microfrontends;
    }
    
    public async Task<bool> PublishAppShellAsync(string appShellDir, Dictionary<string, string> options)
    {
        if (string.IsNullOrEmpty(appShellDir))
            throw new ArgumentNullException(nameof(appShellDir));
            
        if (!Directory.Exists(appShellDir))
            throw new DirectoryNotFoundException($"App shell directory not found: {appShellDir}");
        
        try
        {
            // First package the app shell
            string outputDir = options.TryGetValue("OutputDir", out var outDir) ? outDir : "packages";
            var packagePath = await _packageService.PackageAppShellAsync(appShellDir, outputDir, options);
            
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
} 