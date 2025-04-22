using System.Threading.Tasks;

namespace BlazorMicrofrontends.Core
{
    /// <summary>
    /// Interface for packaging microfrontend modules into distributable packages
    /// </summary>
    public interface IMicrofrontendPackager
    {
        /// <summary>
        /// Creates a package from the microfrontend source directory
        /// </summary>
        /// <param name="sourceDirectory">Directory containing the microfrontend source code</param>
        /// <param name="outputDirectory">Directory where the package will be created</param>
        /// <param name="options">Package options</param>
        /// <returns>Path to the created package</returns>
        Task<string> CreatePackageAsync(string sourceDirectory, string outputDirectory, PackageOptions options);

        /// <summary>
        /// Publishes a package to a feed
        /// </summary>
        /// <param name="packagePath">Path to the package file</param>
        /// <param name="options">Feed options</param>
        /// <returns>True if the package was published successfully</returns>
        Task<bool> PublishPackageAsync(string packagePath, FeedOptions options);
    }
} 