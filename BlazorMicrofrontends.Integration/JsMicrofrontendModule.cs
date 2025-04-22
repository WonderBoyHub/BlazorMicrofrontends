using BlazorMicrofrontends.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorMicrofrontends.Integration;

/// <summary>
/// Represents a microfrontend module implemented using JavaScript-based frameworks (React, Vue, Angular, etc.).
/// </summary>
public class JsMicrofrontendModule : MicrofrontendModuleBase
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _elementId;
    private readonly string _scriptUrl;
    private readonly string _cssUrl;
    private readonly string _mountFunction;
    private readonly string _unmountFunction;
    
    /// <inheritdoc />
    public override string ModuleId { get; }
    
    /// <inheritdoc />
    public override string Name { get; }
    
    /// <inheritdoc />
    public override string Version { get; }
    
    /// <inheritdoc />
    public override string Technology { get; }
    
    /// <summary>
    /// Gets the ID of the element where the microfrontend will be mounted.
    /// </summary>
    public string ElementId => _elementId;
    
    /// <summary>
    /// Gets the URL of the JavaScript script for this module.
    /// </summary>
    public string ScriptUrl => _scriptUrl;
    
    /// <summary>
    /// Gets the URL of the CSS file for this module.
    /// </summary>
    public string CssUrl => _cssUrl;
    
    /// <summary>
    /// Creates a new instance of the <see cref="JsMicrofrontendModule"/> class.
    /// </summary>
    /// <param name="moduleId">The unique identifier for this module.</param>
    /// <param name="name">The display name of the module.</param>
    /// <param name="version">The version of the module.</param>
    /// <param name="technology">The JavaScript framework used (React, Vue, Angular, etc.).</param>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    /// <param name="scriptUrl">The URL of the JavaScript script for this module.</param>
    /// <param name="elementId">The ID of the element where the microfrontend will be mounted.</param>
    /// <param name="mountFunction">The name of the JavaScript function to call to mount the component.</param>
    /// <param name="unmountFunction">The name of the JavaScript function to call to unmount the component.</param>
    /// <param name="cssUrl">The URL of the CSS file for this module, if any.</param>
    public JsMicrofrontendModule(
        string moduleId,
        string name,
        string version,
        string technology,
        IJSRuntime jsRuntime,
        string scriptUrl,
        string elementId,
        string mountFunction = "mount",
        string unmountFunction = "unmount",
        string? cssUrl = null)
    {
        ModuleId = moduleId ?? throw new ArgumentNullException(nameof(moduleId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Technology = technology ?? throw new ArgumentNullException(nameof(technology));
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _scriptUrl = scriptUrl ?? throw new ArgumentNullException(nameof(scriptUrl));
        _elementId = elementId ?? throw new ArgumentNullException(nameof(elementId));
        _mountFunction = mountFunction ?? throw new ArgumentNullException(nameof(mountFunction));
        _unmountFunction = unmountFunction ?? throw new ArgumentNullException(nameof(unmountFunction));
        _cssUrl = cssUrl ?? string.Empty;
    }
    
    /// <inheritdoc />
    protected override async Task OnInitializeAsync()
    {
        try
        {
            // Load the script
            await _jsRuntime.InvokeVoidAsync("eval", $"import('{_scriptUrl}')");
            
            // Load the CSS if specified
            if (!string.IsNullOrEmpty(_cssUrl))
            {
                await _jsRuntime.InvokeVoidAsync("eval", $@"
                    const link = document.createElement('link');
                    link.rel = 'stylesheet';
                    link.href = '{_cssUrl}';
                    document.head.appendChild(link);
                ");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize JavaScript microfrontend module '{ModuleId}': {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Mounts the JavaScript component to the DOM.
    /// </summary>
    /// <param name="props">The props to pass to the component.</param>
    /// <returns>A task representing the asynchronous mount operation.</returns>
    public async Task MountAsync(object? props = null)
    {
        if (!IsInitialized)
        {
            await InitializeAsync();
        }
        
        try
        {
            string propsJson = props != null ? JsonSerializer.Serialize(props) : "null";
            
            await _jsRuntime.InvokeVoidAsync("eval", $@"
                if (window.{Technology} && window.{Technology}.{_mountFunction}) {{
                    window.{Technology}.{_mountFunction}('{_elementId}', {propsJson});
                }}
            ");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to mount JavaScript microfrontend module '{ModuleId}': {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Unmounts the JavaScript component from the DOM.
    /// </summary>
    /// <returns>A task representing the asynchronous unmount operation.</returns>
    public async Task UnmountAsync()
    {
        if (!IsInitialized)
            return;
        
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", $@"
                if (window.{Technology} && window.{Technology}.{_unmountFunction}) {{
                    window.{Technology}.{_unmountFunction}('{_elementId}');
                }}
            ");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to unmount JavaScript microfrontend module '{ModuleId}': {ex.Message}", ex);
        }
    }
} 