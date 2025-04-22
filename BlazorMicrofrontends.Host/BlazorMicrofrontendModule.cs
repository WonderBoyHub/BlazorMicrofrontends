using BlazorMicrofrontends.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorMicrofrontends.Host;

/// <summary>
/// Represents a microfrontend module implemented using Blazor.
/// </summary>
public class BlazorMicrofrontendModule : MicrofrontendModuleBase
{
    private readonly Type _componentType;
    
    /// <summary>
    /// Gets the component type that this module represents.
    /// </summary>
    public Type ComponentType => _componentType;
    
    /// <inheritdoc />
    public override string ModuleId { get; }
    
    /// <inheritdoc />
    public override string Name { get; }
    
    /// <inheritdoc />
    public override string Version { get; }
    
    /// <inheritdoc />
    public override string Technology => "Blazor";
    
    /// <summary>
    /// Gets whether this module represents a Blazor WebAssembly component.
    /// </summary>
    public bool IsWebAssembly { get; }
    
    /// <summary>
    /// Gets the render mode for the Blazor component.
    /// </summary>
    public IComponentRenderMode? RenderMode { get; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="BlazorMicrofrontendModule"/> class.
    /// </summary>
    /// <param name="moduleId">The unique identifier for this module.</param>
    /// <param name="name">The display name of the module.</param>
    /// <param name="version">The version of the module.</param>
    /// <param name="componentType">The type of the Blazor component that this module represents.</param>
    /// <param name="isWebAssembly">Whether this module represents a Blazor WebAssembly component.</param>
    /// <param name="renderMode">The render mode for the Blazor component.</param>
    public BlazorMicrofrontendModule(
        string moduleId,
        string name,
        string version,
        Type componentType,
        bool isWebAssembly = false,
        IComponentRenderMode? renderMode = null)
    {
        if (!typeof(IComponent).IsAssignableFrom(componentType))
        {
            throw new ArgumentException($"The type '{componentType.FullName}' is not a Blazor component.", nameof(componentType));
        }
        
        ModuleId = moduleId ?? throw new ArgumentNullException(nameof(moduleId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        _componentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
        IsWebAssembly = isWebAssembly;
        RenderMode = renderMode;
    }
    
    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        
        // Register the component type
        services.AddTransient(_componentType);
    }
    
    /// <summary>
    /// Creates a render fragment for this Blazor component.
    /// </summary>
    /// <param name="parameters">The parameters to pass to the component.</param>
    /// <returns>A render fragment that represents the component.</returns>
    public RenderFragment CreateComponent(IDictionary<string, object>? parameters = null)
    {
        return builder =>
        {
            builder.OpenComponent(0, _componentType);
            
            if (parameters != null)
            {
                int sequence = 1;
                foreach (var parameter in parameters)
                {
                    builder.AddAttribute(sequence++, parameter.Key, parameter.Value);
                }
            }
            
            builder.CloseComponent();
        };
    }
} 