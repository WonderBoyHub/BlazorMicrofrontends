# Blazor Microfrontends SDK API Reference

This document provides a comprehensive reference to the APIs available in the Blazor Microfrontends SDK.

## Core APIs

### BlazorMicrofrontends.Core

#### `MicrofrontendRegistry`

The central registry for managing microfrontends.

```csharp
public interface IMicrofrontendRegistry
{
    // Retrieves all registered microfrontends
    IEnumerable<MicrofrontendInfo> GetMicrofrontends();
    
    // Registers a new microfrontend
    void RegisterMicrofrontend(MicrofrontendInfo microfrontendInfo);
    
    // Unregisters a microfrontend by ID
    void UnregisterMicrofrontend(string moduleId);
    
    // Gets a microfrontend by ID
    MicrofrontendInfo GetMicrofrontend(string moduleId);
    
    // Checks if a microfrontend is registered
    bool IsMicrofrontendRegistered(string moduleId);
}
```

#### `MicrofrontendInfo`

Information about a registered microfrontend.

```csharp
public class MicrofrontendInfo
{
    // Unique identifier for the microfrontend
    public string ModuleId { get; set; }
    
    // Display name for the microfrontend
    public string Name { get; set; }
    
    // Version of the microfrontend
    public string Version { get; set; }
    
    // Route where the microfrontend is mounted
    public string Route { get; set; }
    
    // Flag indicating if the microfrontend is enabled
    public bool Enabled { get; set; }
    
    // Type of the microfrontend (Blazor, React, Python, BlazorMaui, etc.)
    public string Technology { get; set; }
    
    // Additional metadata for the microfrontend
    public Dictionary<string, string> Metadata { get; set; }
}
```

#### Service Collection Extensions

Extensions for registering core microfrontend services.

```csharp
public static class MicrofrontendServiceCollectionExtensions
{
    // Adds core microfrontend services to the service collection
    public static IServiceCollection AddBlazorMicrofrontendCore(this IServiceCollection services);
    
    // Registers a Blazor microfrontend
    public static IServiceCollection AddBlazorMicrofrontend(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        Type componentType,
        string route = null,
        bool enabled = true,
        Dictionary<string, string> metadata = null);
        
    // Registers a Blazor microfrontend using a generic type parameter
    public static IServiceCollection AddBlazorMicrofrontend<TComponent>(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string route = null,
        bool enabled = true,
        Dictionary<string, string> metadata = null) where TComponent : IComponent;
}
```

### BlazorMicrofrontends.Host

#### `MicrofrontendHost`

Base component for hosting microfrontends.

```csharp
public class MicrofrontendHost : ComponentBase
{
    // The ID of the microfrontend to host
    [Parameter]
    public string ModuleId { get; set; }
    
    // Additional parameters to pass to the microfrontend
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalParameters { get; set; }
    
    // Event callback when the microfrontend is loaded
    [Parameter]
    public EventCallback<MicrofrontendInfo> OnLoaded { get; set; }
    
    // Event callback when the microfrontend fails to load
    [Parameter]
    public EventCallback<Exception> OnError { get; set; }
    
    // Content to display while loading
    [Parameter]
    public RenderFragment LoadingContent { get; set; }
    
    // Content to display on error
    [Parameter]
    public RenderFragment<Exception> ErrorContent { get; set; }
}
```

#### `BlazorMicrofrontendHost`

Component for hosting Blazor microfrontends.

```csharp
public class BlazorMicrofrontendHost : MicrofrontendHost
{
    // Inherits all parameters from MicrofrontendHost
}
```

#### Service Collection Extensions

Extensions for registering host services.

```csharp
public static class HostServiceCollectionExtensions
{
    // Adds microfrontend hosting services to the service collection
    public static IServiceCollection AddBlazorMicrofrontendHost(this IServiceCollection services);
}
```

### BlazorMicrofrontends.Integration

#### `JsMicrofrontendHost`

Component for hosting JavaScript-based microfrontends (React, Vue, etc.).

```csharp
public class JsMicrofrontendHost : MicrofrontendHost
{
    // Inherits all parameters from MicrofrontendHost
    
    // Base URL for loading JavaScript assets
    [Parameter]
    public string BaseUrl { get; set; }
    
    // Root element ID for mounting the JavaScript microfrontend
    [Parameter]
    public string RootElementId { get; set; }
}
```

#### `PythonMicrofrontendHost`

Component for hosting Python-based microfrontends.

```csharp
public class PythonMicrofrontendHost : MicrofrontendHost
{
    // Inherits all parameters from MicrofrontendHost
    
    // Base URL for loading Python assets
    [Parameter]
    public string BaseUrl { get; set; }
    
    // Root element ID for mounting the Python microfrontend
    [Parameter]
    public string RootElementId { get; set; }
    
    // Python runtime options
    [Parameter]
    public PythonRuntimeOptions RuntimeOptions { get; set; }
}
```

#### Service Collection Extensions

Extensions for registering integration services.

```csharp
public static class IntegrationServiceCollectionExtensions
{
    // Adds JavaScript integration services to the service collection
    public static IServiceCollection AddJavaScriptMicrofrontendIntegration(this IServiceCollection services);
    
    // Adds Python integration services to the service collection
    public static IServiceCollection AddPythonMicrofrontendIntegration(this IServiceCollection services);
    
    // Registers a JavaScript microfrontend
    public static IServiceCollection AddJavaScriptMicrofrontend(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string scriptUrl,
        string route = null,
        bool enabled = true,
        Dictionary<string, string> metadata = null);
        
    // Registers a Python microfrontend
    public static IServiceCollection AddPythonMicrofrontend(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string scriptUrl,
        string route = null,
        bool enabled = true,
        Dictionary<string, string> metadata = null);
}
```

### BlazorMicrofrontends.Maui

#### `MauiMicrofrontendHost`

Component for hosting MAUI-based microfrontends.

```csharp
public class MauiMicrofrontendHost : MicrofrontendHost
{
    // Inherits all parameters from MicrofrontendHost
}
```

#### Service Collection Extensions

Extensions for registering MAUI integration services.

```csharp
public static class MauiServiceCollectionExtensions
{
    // Adds MAUI integration services to the service collection
    public static IServiceCollection AddMauiMicrofrontendIntegration(this IServiceCollection services);
    
    // Registers a MAUI microfrontend
    public static IServiceCollection AddMauiMicrofrontend(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string assemblyName,
        string componentTypeName,
        string route = null,
        bool enabled = true,
        Dictionary<string, string> metadata = null);
        
    // Registers a MAUI microfrontend using a generic type parameter
    public static IServiceCollection AddMauiMicrofrontend<TComponent>(
        this IServiceCollection services,
        string moduleId,
        string name,
        string version,
        string route = null,
        bool enabled = true,
        Dictionary<string, string> metadata = null) where TComponent : IComponent;
        
    // Registers a MAUI module in the MAUI application builder
    public static MauiAppBuilder AddMauiMicrofrontendModule<TComponent>(
        this MauiAppBuilder builder,
        string moduleId,
        string name,
        string version,
        Dictionary<string, string> metadata = null) where TComponent : IComponent;
}
```

## CLI Commands

### BlazorMicrofrontends.CLI

#### `NewCommand`

Creates new app shells and microfrontends.

```csharp
// Usage examples:
// bmf new app-shell -n MyAppShell -t EmptyServer
// bmf new microfrontend -n MyComponent -t Blazor
```

#### `AddCommand`

Adds a microfrontend to an app shell.

```csharp
// Usage example:
// bmf add -a ./MyAppShell -m ./MyComponent --route /my-component
```

#### `ListCommand`

Lists available templates, registered microfrontends, or app shell information.

```csharp
// Usage examples:
// bmf list templates
// bmf list microfrontends -a ./MyAppShell
```

#### `PublishCommand`

Publishes app shells or microfrontends as NuGet packages.

```csharp
// Usage examples:
// bmf publish app-shell -p ./MyAppShell -o ./packages
// bmf publish microfrontend -p ./MyComponent -o ./packages -r https://nuget.org -k API_KEY
```

## Configuration

### appsettings.json

Configuration for app shells and microfrontends.

```json
{
  "BlazorMicrofrontends": {
    "Microfrontends": [
      {
        "ModuleId": "counter",
        "Name": "Counter Component",
        "Version": "1.0.0",
        "Route": "/counter",
        "Enabled": true,
        "Technology": "Blazor",
        "Metadata": {
          "Assembly": "CounterComponent.dll",
          "ComponentTypeName": "CounterComponent.Components.Counter"
        }
      },
      {
        "ModuleId": "todo-list",
        "Name": "Todo List",
        "Version": "1.0.0",
        "Route": "/todos",
        "Enabled": true,
        "Technology": "React",
        "Metadata": {
          "ScriptUrl": "/microfrontends/todo-list/main.js",
          "RootElementId": "todo-list-root"
        }
      },
      {
        "ModuleId": "data-visualization",
        "Name": "Data Visualization",
        "Version": "1.0.0",
        "Route": "/visualization",
        "Enabled": true,
        "Technology": "Python",
        "Metadata": {
          "ScriptUrl": "/microfrontends/data-visualization/main.py",
          "RootElementId": "data-viz-root"
        }
      },
      {
        "ModuleId": "device-info",
        "Name": "Device Information",
        "Version": "1.0.0",
        "Route": "/device-info",
        "Enabled": true,
        "Technology": "BlazorMaui",
        "Metadata": {
          "Assembly": "DeviceInfo.dll",
          "ComponentTypeName": "DeviceInfo.Components.DeviceInfoComponent"
        }
      }
    ]
  }
}
```

## Communication APIs

### `MicrofrontendEventBus`

Event bus for communication between microfrontends.

```csharp
public interface IMicrofrontendEventBus
{
    // Publishes an event to all subscribers
    void Publish<TEvent>(TEvent @event) where TEvent : class;
    
    // Subscribes to events of the specified type
    IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
    
    // Subscribes to events of the specified type with a filter
    IDisposable Subscribe<TEvent>(Func<TEvent, bool> filter, Action<TEvent> handler) where TEvent : class;
}
```

### JavaScript Interop

#### `MicrofrontendJsInterop`

Provides JavaScript interop capabilities for microfrontends.

```csharp
public interface IMicrofrontendJsInterop
{
    // Invokes a JavaScript function
    ValueTask<TValue> InvokeAsync<TValue>(string moduleId, string identifier, params object[] args);
    
    // Invokes a JavaScript function without a return value
    ValueTask InvokeVoidAsync(string moduleId, string identifier, params object[] args);
    
    // Registers a .NET method to be called from JavaScript
    void RegisterDotNetMethod<TMethod>(string moduleId, string identifier, TMethod method);
}
```

## Advanced APIs

### Lazy Loading

#### `LazyMicrofrontendRegistry`

Registry for lazily loaded microfrontends.

```csharp
public interface ILazyMicrofrontendRegistry : IMicrofrontendRegistry
{
    // Loads a microfrontend on demand
    Task<MicrofrontendInfo> LoadMicrofrontendAsync(string moduleId);
    
    // Unloads a microfrontend
    Task UnloadMicrofrontendAsync(string moduleId);
    
    // Checks if a microfrontend is loaded
    bool IsMicrofrontendLoaded(string moduleId);
}
```

### Authentication

#### `MicrofrontendAuthenticationService`

Service for handling authentication in microfrontends.

```csharp
public interface IMicrofrontendAuthenticationService
{
    // Gets the current user
    Task<MicrofrontendUser> GetUserAsync();
    
    // Checks if the user is authenticated
    Task<bool> IsAuthenticatedAsync();
    
    // Checks if the user has a specific role
    Task<bool> IsInRoleAsync(string role);
    
    // Checks if the user has a specific permission
    Task<bool> HasPermissionAsync(string permission);
}
```

### State Management

#### `MicrofrontendStateManager`

Service for managing shared state between microfrontends.

```csharp
public interface IMicrofrontendStateManager
{
    // Gets a state value by key
    TState GetState<TState>(string key);
    
    // Sets a state value
    void SetState<TState>(string key, TState value);
    
    // Subscribes to state changes
    IDisposable SubscribeToState<TState>(string key, Action<TState> handler);
    
    // Removes a state value
    void RemoveState(string key);
}
``` 