using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorMicrofrontends.CLI.Models;

namespace BlazorMicrofrontends.CLI.Services;

/// <summary>
/// Service for packaging and publishing microfrontends
/// </summary>
public interface IPackageService
{
    /// <summary>
    /// Packages a microfrontend for distribution
    /// </summary>
    /// <param name="microfrontendDir">Directory of the microfrontend to package</param>
    /// <param name="outputDir">Output directory for the package</param>
    /// <param name="options">Packaging options</param>
    /// <returns>Path to the created package</returns>
    Task<string> PackageMicrofrontendAsync(string microfrontendDir, string outputDir, Dictionary<string, string> options);
    
    /// <summary>
    /// Packages an app shell for distribution
    /// </summary>
    /// <param name="appShellDir">Directory of the app shell to package</param>
    /// <param name="outputDir">Output directory for the package</param>
    /// <param name="options">Packaging options</param>
    /// <returns>Path to the created package</returns>
    Task<string> PackageAppShellAsync(string appShellDir, string outputDir, Dictionary<string, string> options);
    
    /// <summary>
    /// Publishes a package to a feed
    /// </summary>
    /// <param name="packagePath">Path to the package to publish</param>
    /// <param name="options">Publishing options including feed URL, API key, etc.</param>
    /// <returns>True if published successfully</returns>
    Task<bool> PublishPackageAsync(string packagePath, Dictionary<string, string> options);
    
    /// <summary>
    /// Lists packages available in a feed
    /// </summary>
    /// <param name="feedUrl">URL of the feed</param>
    /// <param name="options">Options for listing packages</param>
    /// <returns>Collection of package information</returns>
    Task<IEnumerable<PackageInfo>> ListPackagesAsync(string feedUrl, Dictionary<string, string> options);
    
    /// <summary>
    /// Lists available packages from a repository
    /// </summary>
    /// <param name="repositoryUrl">URL of the repository</param>
    /// <param name="searchTerm">Optional search term to filter packages</param>
    /// <returns>Collection of package IDs</returns>
    Task<IEnumerable<string>> ListAvailablePackagesAsync(string repositoryUrl, string searchTerm);
} 