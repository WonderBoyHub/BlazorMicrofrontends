using System.Collections.Generic;

namespace BlazorMicrofrontends.Core
{
    /// <summary>
    /// Options for creating a microfrontend package
    /// </summary>
    public class PackageOptions
    {
        /// <summary>
        /// Gets or sets the package id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the package version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the package authors
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// Gets or sets the package description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the package tags
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets additional metadata for the package
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
} 