namespace BlazorMicrofrontends.CLI.Services;

/// <summary>
/// Service for managing templates used to create app shells and microfrontends
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Gets the list of available app shell templates
    /// </summary>
    Task<IEnumerable<string>> GetAppShellTemplatesAsync();
    
    /// <summary>
    /// Gets the list of available microfrontend templates
    /// </summary>
    Task<IEnumerable<string>> GetMicrofrontendTemplatesAsync();
    
    /// <summary>
    /// Creates a new project from a template
    /// </summary>
    /// <param name="templateName">Name of the template</param>
    /// <param name="outputDirectory">Directory to create the project in</param>
    /// <param name="projectName">Name of the project</param>
    /// <param name="options">Additional template options</param>
    Task<bool> CreateFromTemplateAsync(string templateName, string outputDirectory, string projectName, Dictionary<string, string> options);
} 