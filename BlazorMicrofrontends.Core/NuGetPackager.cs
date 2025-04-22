// BlazorMicrofrontends.Core/Packaging/NuGetPackager.cs
using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BlazorMicrofrontends.Core
{
    /// <summary>
    /// Implements packaging and publishing for microfrontends using NuGet
    /// </summary>
    public class NuGetPackager : IMicrofrontendPackager
    {
        /// <summary>
        /// Creates a package from the microfrontend source directory
        /// </summary>
        /// <param name="sourceDirectory">Directory containing the microfrontend source code</param>
        /// <param name="outputDirectory">Directory where the package will be created</param>
        /// <param name="options">Package options</param>
        /// <returns>Path to the created package</returns>
        public async Task<string> CreatePackageAsync(string sourceDirectory, string outputDirectory, PackageOptions options)
        {
            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArgumentNullException(nameof(sourceDirectory));
                
            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentNullException(nameof(outputDirectory));
                
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            // Create output directory if it doesn't exist
            Directory.CreateDirectory(outputDirectory);
            
            // Find the project file (.csproj, .fsproj, etc.)
            var projectFiles = Directory.GetFiles(sourceDirectory, "*.csproj");
            if (!projectFiles.Any())
                projectFiles = Directory.GetFiles(sourceDirectory, "*.fsproj");
            if (!projectFiles.Any())
                projectFiles = Directory.GetFiles(sourceDirectory, "*.vbproj");
                
            if (!projectFiles.Any())
                throw new FileNotFoundException("No project file found in the source directory");
                
            var projectFile = projectFiles.First();
            
            // Build the dotnet pack command
            var args = $"pack \"{projectFile}\" -o \"{outputDirectory}\"";
            
            // Add version if specified
            if (!string.IsNullOrEmpty(options.Version))
                args += $" -p:Version={options.Version}";
                
            // Add configuration (Release by default)
            args += " -c Release";
            
            // Execute the command
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            if (process.ExitCode != 0)
                throw new Exception($"Failed to create package: {error}");
                
            // Get the path to the created package
            var packageName = Path.GetFileNameWithoutExtension(projectFile);
            var version = options.Version ?? "1.0.0"; // Default version if not specified
            var packagePath = Path.Combine(outputDirectory, $"{packageName}.{version}.nupkg");
            
            if (!File.Exists(packagePath))
                throw new FileNotFoundException($"Package file not found: {packagePath}");
                
            return packagePath;
        }

        /// <summary>
        /// Publishes a package to a NuGet feed
        /// </summary>
        /// <param name="packagePath">Path to the package file</param>
        /// <param name="options">Feed options</param>
        /// <returns>True if the package was published successfully</returns>
        public async Task<bool> PublishPackageAsync(string packagePath, FeedOptions options)
        {
            if (string.IsNullOrEmpty(packagePath))
                throw new ArgumentNullException(nameof(packagePath));
                
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            if (string.IsNullOrEmpty(options.FeedUrl))
                throw new ArgumentException("Feed URL is required", nameof(options.FeedUrl));
                
            if (!File.Exists(packagePath))
                throw new FileNotFoundException($"Package file not found: {packagePath}");
                
            // Build the dotnet nuget push command
            var args = $"nuget push \"{packagePath}\" --source \"{options.FeedUrl}\"";
            
            // Add API key if specified
            if (!string.IsNullOrEmpty(options.ApiKey))
                args += $" --api-key {options.ApiKey}";
                
            // Add skip duplicate option if specified
            if (options.SkipDuplicate)
                args += " --skip-duplicate";
                
            // Execute the command
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            return process.ExitCode == 0;
        }
    }
}