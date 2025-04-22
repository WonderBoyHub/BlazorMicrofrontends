using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorMicrofrontends.CLI.Models;
using Spectre.Console;

namespace BlazorMicrofrontends.CLI.Services.Implementations;

public class PackageService : IPackageService
{
    private readonly ITemplateService _templateService;
    
    public PackageService(ITemplateService templateService)
    {
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
    }
    
    public async Task<string> PackageMicrofrontendAsync(string microfrontendDir, string outputDir, Dictionary<string, string> options)
    {
        if (string.IsNullOrEmpty(microfrontendDir))
            throw new ArgumentNullException(nameof(microfrontendDir));
            
        if (string.IsNullOrEmpty(outputDir))
            throw new ArgumentNullException(nameof(outputDir));
            
        if (!Directory.Exists(microfrontendDir))
            throw new DirectoryNotFoundException($"Microfrontend directory not found: {microfrontendDir}");
            
        // Create output directory if it doesn't exist
        Directory.CreateDirectory(outputDir);
        
        // Determine the type of microfrontend technology to select appropriate packager
        string technology = DetermineTechnology(microfrontendDir);
        AnsiConsole.WriteLine($"Detected microfrontend technology: {technology}");
        
        // Package based on the technology
        string packagePath = await PackageByTechnologyAsync(technology, microfrontendDir, outputDir, options);
        
        return packagePath;
    }
    
    public async Task<string> PackageAppShellAsync(string appShellDir, string outputDir, Dictionary<string, string> options)
    {
        if (string.IsNullOrEmpty(appShellDir))
            throw new ArgumentNullException(nameof(appShellDir));
            
        if (string.IsNullOrEmpty(outputDir))
            throw new ArgumentNullException(nameof(outputDir));
            
        if (!Directory.Exists(appShellDir))
            throw new DirectoryNotFoundException($"App shell directory not found: {appShellDir}");
            
        // Create output directory if it doesn't exist
        Directory.CreateDirectory(outputDir);
        
        // For app shells, we primarily use NuGet packaging
        string packagePath = await PackageDotNetProjectAsync(appShellDir, outputDir, options);
        
        return packagePath;
    }
    
    public async Task<bool> PublishPackageAsync(string packagePath, Dictionary<string, string> options)
    {
        if (string.IsNullOrEmpty(packagePath))
            throw new ArgumentNullException(nameof(packagePath));
            
        if (!File.Exists(packagePath))
            throw new FileNotFoundException($"Package file not found: {packagePath}");
        
        // Get feed URL and API key from options
        if (!options.TryGetValue("feed", out string feedUrl))
        {
            throw new ArgumentException("Feed URL is required in options");
        }
        
        options.TryGetValue("apiKey", out string apiKey);
        
        // Determine package type
        string packageType = DeterminePackageType(packagePath);
        
        switch (packageType)
        {
            case "nuget":
                return await PublishNuGetPackageAsync(packagePath, feedUrl, apiKey);
            case "npm":
                return await PublishNpmPackageAsync(packagePath, feedUrl, apiKey);
            case "wheel":
                return await PublishPythonPackageAsync(packagePath, feedUrl, apiKey);
            default:
                throw new NotSupportedException($"Package type '{packageType}' is not supported");
        }
    }
    
    public async Task<IEnumerable<PackageInfo>> ListPackagesAsync(string feedUrl, Dictionary<string, string> options)
    {
        if (string.IsNullOrEmpty(feedUrl))
            throw new ArgumentNullException(nameof(feedUrl));
        
        // Determine feed type from URL or options
        string feedType = DetermineFeedType(feedUrl, options);
        
        options.TryGetValue("apiKey", out string apiKey);
        options.TryGetValue("searchTerm", out string searchTerm);
        
        switch (feedType)
        {
            case "nuget":
                return await ListNuGetPackagesAsync(feedUrl, apiKey, searchTerm);
            case "npm":
                return await ListNpmPackagesAsync(feedUrl, apiKey, searchTerm);
            case "pypi":
                return await ListPythonPackagesAsync(feedUrl, apiKey, searchTerm);
            default:
                throw new NotSupportedException($"Feed type '{feedType}' is not supported");
        }
    }
    
    public async Task<IEnumerable<string>> ListAvailablePackagesAsync(string repositoryUrl, string searchTerm)
    {
        if (string.IsNullOrEmpty(repositoryUrl))
            throw new ArgumentNullException(nameof(repositoryUrl));
        
        // Determine feed type from URL
        string feedType = DetermineFeedType(repositoryUrl, new Dictionary<string, string>());
        
        var options = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            options["searchTerm"] = searchTerm;
        }
        
        // Get packages from the feed
        var packages = await ListPackagesAsync(repositoryUrl, options);
        
        // Extract just the IDs and return
        return packages.Select(p => p.Id).ToList();
    }
    
    #region Private Helper Methods
    
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
    
    private string DeterminePackageType(string packagePath)
    {
        string extension = Path.GetExtension(packagePath).ToLowerInvariant();
        
        switch (extension)
        {
            case ".nupkg":
                return "nuget";
            case ".tgz":
                return "npm";
            case ".whl":
                return "wheel";
            default:
                throw new NotSupportedException($"Package type for extension '{extension}' is not supported");
        }
    }
    
    private string DetermineFeedType(string feedUrl, Dictionary<string, string> options)
    {
        // Check if feed type is explicitly specified
        if (options.TryGetValue("feedType", out string feedType))
            return feedType;
            
        // Try to determine from the URL
        if (feedUrl.Contains("nuget.org") || feedUrl.Contains(".nuget."))
            return "nuget";
            
        if (feedUrl.Contains("npmjs.org") || feedUrl.Contains("npm."))
            return "npm";
            
        if (feedUrl.Contains("pypi.org") || feedUrl.Contains("pypi."))
            return "pypi";
            
        if (feedUrl.Contains("dev.azure.com") || feedUrl.Contains("visualstudio.com"))
            return "azuredevops";
            
        if (feedUrl.Contains("github.com") || feedUrl.Contains("pkg.github.com"))
            return "github";
            
        // Default to custom feed type
        return "custom";
    }
    
    private async Task<string> PackageByTechnologyAsync(string technology, string sourcePath, string outputDir, Dictionary<string, string> options)
    {
        switch (technology)
        {
            case "Blazor":
            case "BlazorMaui":
                return await PackageDotNetProjectAsync(sourcePath, outputDir, options);
            case "React":
            case "JavaScript":
                return await PackageJavaScriptProjectAsync(sourcePath, outputDir, options);
            case "Python":
                return await PackagePythonProjectAsync(sourcePath, outputDir, options);
            default:
                throw new NotSupportedException($"Technology '{technology}' is not supported for packaging");
        }
    }
    
    private async Task<string> PackageDotNetProjectAsync(string projectPath, string outputDir, Dictionary<string, string> options)
    {
        AnsiConsole.WriteLine("Packaging .NET project...");
        
        // Find the project file
        var projectFiles = Directory.GetFiles(projectPath, "*.csproj");
        if (!projectFiles.Any())
            throw new FileNotFoundException("No .csproj file found in the specified directory");
            
        var projectFile = projectFiles.First();
        var projectName = Path.GetFileNameWithoutExtension(projectFile);
        
        // Extract package version from options
        string version = "1.0.0";
        if (options.TryGetValue("version", out string ver))
            version = ver;
            
        // Build pack command
        string args = $"pack \"{projectFile}\" -o \"{outputDir}\" -p:Version={version} -c Release";
        
        // Add any additional options
        if (options.TryGetValue("configuration", out string config))
            args = args.Replace("-c Release", $"-c {config}");
            
        AnsiConsole.WriteLine($"Running: dotnet {args}");
        
        // Execute the command
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
        process.Start();
        
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();
        
        if (process.ExitCode != 0)
        {
            AnsiConsole.WriteException(new Exception($"Failed to package .NET project. Error: {error}"));
            throw new Exception($"Failed to package .NET project. Error: {error}");
        }
        
        // Find the created package
        var packageFiles = Directory.GetFiles(outputDir, $"{projectName}.{version}.nupkg");
        if (!packageFiles.Any())
            throw new FileNotFoundException("No package file was created");
            
        string packagePath = packageFiles.First();
        AnsiConsole.WriteLine($"Package created at: {packagePath}");
        
        return packagePath;
    }
    
    private async Task<string> PackageJavaScriptProjectAsync(string projectPath, string outputDir, Dictionary<string, string> options)
    {
        AnsiConsole.WriteLine("Packaging JavaScript/React project...");
        
        // Find package.json
        var packageJsonPath = Path.Combine(projectPath, "package.json");
        if (!File.Exists(packageJsonPath))
            throw new FileNotFoundException("No package.json file found in the specified directory");
            
        // Read package info
        var packageJson = File.ReadAllText(packageJsonPath);
        var packageInfo = JsonSerializer.Deserialize<JsonElement>(packageJson);
        
        string packageName = packageInfo.GetProperty("name").GetString();
        string version = packageInfo.GetProperty("version").GetString();
        
        // Override version if specified in options
        if (options.TryGetValue("version", out string ver))
        {
            version = ver;
            
            // Update version in package.json
            var packageObj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(packageJson);
            packageObj["version"] = JsonSerializer.SerializeToElement(version);
            File.WriteAllText(packageJsonPath, JsonSerializer.Serialize(packageObj, new JsonSerializerOptions { WriteIndented = true }));
        }
        
        // Run npm pack
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "npm",
            Arguments = "pack",
            WorkingDirectory = projectPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
        process.Start();
        
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();
        
        if (process.ExitCode != 0)
        {
            AnsiConsole.WriteException(new Exception($"Failed to package JavaScript project. Error: {error}"));
            throw new Exception($"Failed to package JavaScript project. Error: {error}");
        }
        
        // Find the created package
        string packageFileName = $"{packageName}-{version}.tgz";
        string sourceTgzPath = Path.Combine(projectPath, packageFileName);
        
        if (!File.Exists(sourceTgzPath))
            throw new FileNotFoundException($"Package file not found: {sourceTgzPath}");
            
        // Move to output directory
        string targetTgzPath = Path.Combine(outputDir, packageFileName);
        File.Copy(sourceTgzPath, targetTgzPath, true);
        File.Delete(sourceTgzPath);
        
        AnsiConsole.WriteLine($"Package created at: {targetTgzPath}");
        
        return targetTgzPath;
    }
    
    private async Task<string> PackagePythonProjectAsync(string projectPath, string outputDir, Dictionary<string, string> options)
    {
        AnsiConsole.WriteLine("Packaging Python project...");
        
        // Check for setup.py or pyproject.toml
        var setupPyPath = Path.Combine(projectPath, "setup.py");
        var pyprojectTomlPath = Path.Combine(projectPath, "pyproject.toml");
        
        bool hasSetupPy = File.Exists(setupPyPath);
        bool hasPyprojectToml = File.Exists(pyprojectTomlPath);
        
        if (!hasSetupPy && !hasPyprojectToml)
        {
            // Create minimal setup.py
            var setupPy = CreateMinimalSetupPy(projectPath, options);
            File.WriteAllText(setupPyPath, setupPy);
            hasSetupPy = true;
        }
        
        // Run python setup.py bdist_wheel
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "python",
            Arguments = "-m pip wheel --no-deps -w dist .",
            WorkingDirectory = projectPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
        process.Start();
        
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();
        
        if (process.ExitCode != 0)
        {
            AnsiConsole.WriteException(new Exception($"Failed to package Python project. Error: {error}"));
            throw new Exception($"Failed to package Python project. Error: {error}");
        }
        
        // Find the created package
        var distDir = Path.Combine(projectPath, "dist");
        var wheelFiles = Directory.GetFiles(distDir, "*.whl");
        
        if (!wheelFiles.Any())
            throw new FileNotFoundException("No wheel file was created");
            
        string wheelPath = wheelFiles.First();
        string targetWheelPath = Path.Combine(outputDir, Path.GetFileName(wheelPath));
        
        // Copy to output directory
        File.Copy(wheelPath, targetWheelPath, true);
        
        AnsiConsole.WriteLine($"Package created at: {targetWheelPath}");
        
        return targetWheelPath;
    }
    
    private string CreateMinimalSetupPy(string projectPath, Dictionary<string, string> options)
    {
        string projectName = Path.GetFileName(projectPath);
        string version = "1.0.0";
        
        if (options.TryGetValue("version", out string ver))
            version = ver;
            
        if (options.TryGetValue("name", out string name))
            projectName = name;
            
        return $@"
from setuptools import setup, find_packages

setup(
    name='{projectName}',
    version='{version}',
    description='Python Microfrontend for Blazor Microfrontends SDK',
    author='{options.GetValueOrDefault("author", "Blazor Microfrontends")}',
    packages=find_packages(),
    include_package_data=True,
    install_requires=[
        'flask',
        'pyodide-http'
    ],
)";
    }
    
    private async Task<bool> PublishNuGetPackageAsync(string packagePath, string feedUrl, string apiKey)
    {
        AnsiConsole.WriteLine($"Publishing NuGet package to {feedUrl}...");
        
        string args = $"nuget push \"{packagePath}\" --source \"{feedUrl}\"";
        
        if (!string.IsNullOrEmpty(apiKey))
            args += $" --api-key {apiKey}";
            
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
        process.Start();
        
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();
        
        if (process.ExitCode != 0)
        {
            AnsiConsole.WriteException(new Exception($"Failed to publish NuGet package. Error: {error}"));
            return false;
        }
        
        AnsiConsole.WriteLine("Package published successfully!");
        return true;
    }
    
    private async Task<bool> PublishNpmPackageAsync(string packagePath, string feedUrl, string apiKey)
    {
        AnsiConsole.WriteLine($"Publishing NPM package to {feedUrl}...");
        
        string args = $"publish \"{packagePath}\" --registry={feedUrl}";
        
        if (!string.IsNullOrEmpty(apiKey))
        {
            // Create .npmrc file with auth token
            string npmrcPath = Path.Combine(Path.GetTempPath(), ".npmrc");
            File.WriteAllText(npmrcPath, $"//{new Uri(feedUrl).Host}/:_authToken={apiKey}");
            
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "npm",
                Arguments = $"config --userconfig {npmrcPath} {args}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
            process.Start();
            
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            // Delete temporary .npmrc
            File.Delete(npmrcPath);
            
            if (process.ExitCode != 0)
            {
                AnsiConsole.WriteException(new Exception($"Failed to publish NPM package. Error: {error}"));
                return false;
            }
        }
        else
        {
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "npm",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
            process.Start();
            
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            if (process.ExitCode != 0)
            {
                AnsiConsole.WriteException(new Exception($"Failed to publish NPM package. Error: {error}"));
                return false;
            }
        }
        
        AnsiConsole.WriteLine("Package published successfully!");
        return true;
    }
    
    private async Task<bool> PublishPythonPackageAsync(string packagePath, string feedUrl, string apiKey)
    {
        AnsiConsole.WriteLine($"Publishing Python package to {feedUrl}...");
        
        string args = $"-m twine upload --repository-url {feedUrl} \"{packagePath}\"";
        
        if (!string.IsNullOrEmpty(apiKey))
            args += $" --username __token__ --password {apiKey}";
            
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "python",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
        process.Start();
        
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();
        
        if (process.ExitCode != 0)
        {
            AnsiConsole.WriteException(new Exception($"Failed to publish Python package. Error: {error}"));
            return false;
        }
        
        AnsiConsole.WriteLine("Package published successfully!");
        return true;
    }
    
    private async Task<IEnumerable<PackageInfo>> ListNuGetPackagesAsync(string feedUrl, string apiKey, string searchTerm)
    {
        // For now, we'll just return some sample packages
        // In a real implementation, we would use NuGet.Protocol to search the feed
        
        // Create a sample list of packages
        var packages = new List<PackageInfo>
        {
            new PackageInfo
            {
                Id = "BlazorMicrofrontends.Core",
                Version = "1.0.0",
                Description = "Core library for Blazor Microfrontends",
                Authors = "Your Organization",
                Published = DateTime.Now.AddDays(-10),
                DownloadCount = 250,
                PackageType = "nuget",
                Tags = new[] { "blazor", "microfrontend", "webassembly" },
                Path = string.Empty // Add this to satisfy the required property
            },
            new PackageInfo
            {
                Id = "BlazorMicrofrontends.Host",
                Version = "1.0.0",
                Description = "Host library for Blazor Microfrontends",
                Authors = "Your Organization",
                Published = DateTime.Now.AddDays(-5),
                DownloadCount = 120,
                PackageType = "nuget",
                Tags = new[] { "blazor", "microfrontend", "host" },
                Path = string.Empty // Add this to satisfy the required property
            }
        };
        
        return packages;
    }
    
    private async Task<IEnumerable<PackageInfo>> ListNpmPackagesAsync(string feedUrl, string apiKey, string searchTerm)
    {
        // In a real implementation, we would use the npm registry API to search for packages
        
        // Create a sample list of packages
        var packages = new List<PackageInfo>
        {
            new PackageInfo
            {
                Id = "react-microfrontend",
                Version = "1.0.0",
                Description = "React implementation for microfrontends",
                Authors = "Your Organization",
                Published = DateTime.Now.AddDays(-15),
                DownloadCount = 300,
                PackageType = "npm",
                Tags = new[] { "react", "microfrontend", "javascript" },
                Path = string.Empty // Add this to satisfy the required property
            }
        };
        
        return packages;
    }
    
    private async Task<IEnumerable<PackageInfo>> ListPythonPackagesAsync(string feedUrl, string apiKey, string searchTerm)
    {
        // In a real implementation, we would use the PyPI API to search for packages
        
        // Create a sample list of packages
        var packages = new List<PackageInfo>
        {
            new PackageInfo
            {
                Id = "pyscript-microfrontend",
                Version = "1.0.0",
                Description = "PyScript implementation for microfrontends",
                Authors = "Your Organization",
                Published = DateTime.Now.AddDays(-20),
                DownloadCount = 50,
                PackageType = "wheel",
                Tags = new[] { "python", "pyscript", "microfrontend" },
                Path = string.Empty // Add this to satisfy the required property
            }
        };
        
        return packages;
    }
    
    #endregion
} 