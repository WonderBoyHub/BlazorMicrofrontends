# Getting Started with Blazor Microfrontends SDK

This guide will help you get started with the Blazor Microfrontends SDK, from setting up your environment to creating your first microfrontend application.

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (recommended) or [Visual Studio Code](https://code.visualstudio.com/) with C# extensions
- [Node.js](https://nodejs.org/) (required for JavaScript microfrontends)
- [Python 3.8+](https://www.python.org/downloads/) (required for Python microfrontends)

## Installation

### 1. Install the Blazor Microfrontends CLI tool

```bash
dotnet tool install --global BlazorMicrofrontends.CLI
```

This installs the `bmf` command-line tool that you'll use to create and manage microfrontend projects.

### 2. Verify the installation

```bash
bmf --version
```

You should see the version number of the Blazor Microfrontends CLI tool.

## Creating Your First Microfrontend Application

### 1. Create an App Shell

The app shell is the host application that will contain your microfrontends.

```bash
bmf new app-shell -n MyFirstAppShell -t ServerWithNavigation
```

This creates a new Blazor Server application with navigation and all the necessary configurations to host microfrontends.

### 2. Navigate to the app shell directory

```bash
cd MyFirstAppShell
```

### 3. Create a Blazor microfrontend

```bash
bmf new microfrontend -n Counter -t Blazor
```

This creates a new Blazor microfrontend component that can be added to your app shell.

### 4. Add the microfrontend to your app shell

```bash
bmf add -a . -m ./Counter --route /counter
```

This adds the Counter microfrontend to your app shell with a route of `/counter`.

### 5. Run the application

```bash
dotnet run
```

Open your browser and navigate to `https://localhost:5001/counter` to see your microfrontend in action.

## Project Structure

After following the steps above, your project structure should look like this:

```
MyFirstAppShell/
  ├── wwwroot/
  ├── Components/
  ├── Pages/
  ├── Services/
  ├── Microfrontends/
  │   └── Counter/
  │       ├── Components/
  │       │   └── CounterComponent.razor
  │       └── Counter.csproj
  ├── Program.cs
  ├── appsettings.json
  └── MyFirstAppShell.csproj
```

## Understanding the Code

### App Shell Configuration

The app shell's `Program.cs` file contains the configuration for hosting microfrontends:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddServerSideBlazor();

// Add microfrontend services
builder.Services.AddBlazorMicrofrontendCore();
builder.Services.AddBlazorMicrofrontendHost();

// Register microfrontends from configuration
builder.Services.AddMicrofrontendsFromConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
```

### Microfrontend Configuration in appsettings.json

The app shell's `appsettings.json` file contains the configuration for the microfrontends:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
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
          "Assembly": "Counter.dll",
          "ComponentTypeName": "Counter.Components.CounterComponent"
        }
      }
    ]
  }
}
```

### Using the MicrofrontendHost Component

In your app shell's Razor pages, you can use the `MicrofrontendHost` component to display microfrontends:

```razor
@page "/counter"

<h1>Counter Microfrontend</h1>

<MicrofrontendHost ModuleId="counter" />
```

## Working with Different Types of Microfrontends

### Creating a React Microfrontend

1. Create a new React microfrontend:

```bash
bmf new microfrontend -n TodoList -t React
```

2. Add it to your app shell:

```bash
bmf add -a . -m ./TodoList --route /todos
```

3. The React microfrontend will be registered in `appsettings.json`:

```json
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
}
```

4. Use the JS microfrontend host in your Razor page:

```razor
@page "/todos"

<h1>Todo List Microfrontend</h1>

<JsMicrofrontendHost ModuleId="todo-list" RootElementId="todo-list-root" />
```

### Creating a Python Microfrontend

1. Create a new Python microfrontend:

```bash
bmf new microfrontend -n DataViz -t Python
```

2. Add it to your app shell:

```bash
bmf add -a . -m ./DataViz --route /visualization
```

3. The Python microfrontend will be registered in `appsettings.json`:

```json
{
  "ModuleId": "data-viz",
  "Name": "Data Visualization",
  "Version": "1.0.0",
  "Route": "/visualization",
  "Enabled": true,
  "Technology": "Python",
  "Metadata": {
    "ScriptUrl": "/microfrontends/data-viz/main.py",
    "RootElementId": "data-viz-root"
  }
}
```

4. Use the Python microfrontend host in your Razor page:

```razor
@page "/visualization"

<h1>Data Visualization Microfrontend</h1>

<PythonMicrofrontendHost ModuleId="data-viz" RootElementId="data-viz-root" />
```

## Communication Between Microfrontends

### Using the EventBus

1. Register the event bus service in your app shell's `Program.cs`:

```csharp
builder.Services.AddMicrofrontendEventBus();
```

2. Define an event class:

```csharp
public class ItemSelectedEvent
{
    public string ItemId { get; set; }
    public string ItemName { get; set; }
}
```

3. Publish an event from one microfrontend:

```csharp
@inject IMicrofrontendEventBus EventBus

<button @onclick="SelectItem">Select Item</button>

@code {
    private void SelectItem()
    {
        EventBus.Publish(new ItemSelectedEvent
        {
            ItemId = "123",
            ItemName = "Sample Item"
        });
    }
}
```

4. Subscribe to the event in another microfrontend:

```csharp
@inject IMicrofrontendEventBus EventBus
@implements IDisposable

<p>Selected Item: @selectedItemName</p>

@code {
    private string selectedItemName = "None";
    private IDisposable subscription;

    protected override void OnInitialized()
    {
        subscription = EventBus.Subscribe<ItemSelectedEvent>(e =>
        {
            selectedItemName = e.ItemName;
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

### Using State Management

1. Register the state manager service in your app shell's `Program.cs`:

```csharp
builder.Services.AddMicrofrontendStateManager();
```

2. Set a state value in one microfrontend:

```csharp
@inject IMicrofrontendStateManager StateManager

<input @bind="userName" />
<button @onclick="SaveUserName">Save</button>

@code {
    private string userName;

    private void SaveUserName()
    {
        StateManager.SetState("userName", userName);
    }
}
```

3. Get a state value in another microfrontend:

```csharp
@inject IMicrofrontendStateManager StateManager
@implements IDisposable

<p>Welcome, @userName!</p>

@code {
    private string userName = "Guest";
    private IDisposable subscription;

    protected override void OnInitialized()
    {
        userName = StateManager.GetState<string>("userName") ?? "Guest";
        
        subscription = StateManager.SubscribeToState<string>("userName", value =>
        {
            userName = value ?? "Guest";
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }
}
```

## Deploying Microfrontend Applications

### Publishing as NuGet Packages

1. Publish an app shell as a NuGet package:

```bash
bmf publish app-shell -p ./MyFirstAppShell -o ./packages
```

2. Publish a microfrontend as a NuGet package:

```bash
bmf publish microfrontend -p ./Counter -o ./packages
```

### Containerizing Your Application

You can containerize your microfrontend application using Docker:

1. Create a Dockerfile in your app shell directory:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MyFirstAppShell/MyFirstAppShell.csproj", "MyFirstAppShell/"]
COPY ["Microfrontends/Counter/Counter.csproj", "Microfrontends/Counter/"]
RUN dotnet restore "MyFirstAppShell/MyFirstAppShell.csproj"
COPY . .
WORKDIR "/src/MyFirstAppShell"
RUN dotnet build "MyFirstAppShell.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyFirstAppShell.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyFirstAppShell.dll"]
```

2. Build and run your Docker container:

```bash
docker build -t myfirstappshell .
docker run -p 8080:80 myfirstappshell
```

## Next Steps

Now that you've created your first microfrontend application, you can:

- Explore more advanced features of the Blazor Microfrontends SDK in the [API Reference](api-reference.md)
- Learn about integrating with Blazor MAUI in the [MAUI Integration Guide](maui-integration.md)
- Implement authentication and authorization for your microfrontends
- Create complex microfrontend architectures with multiple technologies

## Troubleshooting

### Common Issues

#### Microfrontend Not Loading

- Check that the microfrontend is correctly registered in `appsettings.json`
- Verify that the assembly name and component type name are correct
- Check for any JavaScript console errors (for JS microfrontends)

#### Routing Issues

- Ensure that the route in `appsettings.json` matches the route in your Razor page
- Check that you have a page set up with the correct route parameter

#### Communication Issues

- Verify that the event bus or state manager is registered in the service collection
- Check that the event types match between the publisher and subscriber

## Getting Help

If you encounter any issues not covered in this guide, you can:

- Check the [documentation](https://github.com/yourusername/BlazorMicrofrontends/wiki)
- Open an issue on [GitHub](https://github.com/yourusername/BlazorMicrofrontends/issues)
- Join the community [Discord](https://discord.gg/blazormicrofrontends) 