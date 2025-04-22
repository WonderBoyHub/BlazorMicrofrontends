using System;

namespace BlazorMicrofrontends.CLI.Models;

/// <summary>
/// Information about a package in a feed
/// </summary>
public class PackageInfo
{
    /// <summary>
    /// Package identifier
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Package version
    /// </summary>
    public required string Version { get; set; }
    
    /// <summary>
    /// Package description
    /// </summary>
    public required string Description { get; set; }
    
    /// <summary>
    /// Package authors
    /// </summary>
    public required string Authors { get; set; }
    
    /// <summary>
    /// Package creation date
    /// </summary>
    public DateTime? Published { get; set; }
    
    /// <summary>
    /// Download count for the package
    /// </summary>
    public long? DownloadCount { get; set; }
    
    /// <summary>
    /// Package type (nuget, npm, wheel, etc.)
    /// </summary>
    public required string PackageType { get; set; }
    
    /// <summary>
    /// Tags associated with the package
    /// </summary>
    public required string[] Tags { get; set; }
    
    /// <summary>
    /// Local path to the package file (if downloaded)
    /// </summary>
    public required string Path { get; set; }
} 