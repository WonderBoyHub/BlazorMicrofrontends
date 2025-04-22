# Blazor Microfrontends SDK Release Guide

This guide explains how to use the Blazor Microfrontends SDK in your projects.

## Available Packages

The Blazor Microfrontends SDK consists of the following NuGet packages:

1. `BlazorMicrofrontends.Core` - Core library for building composable microfrontend applications
2. `BlazorMicrofrontends.Host` - Host library for loading and managing microfrontends
3. `BlazorMicrofrontends.Integration` - Integration library for connecting microfrontends with the app shell
4. `BlazorMicrofrontends.AppShell` - Components and services for building app shells
5. `BlazorMicrofrontends.CLI` - Command-line tool for scaffolding and managing projects
6. `BlazorMicrofrontends.Templates` - Project templates for creating app shells and microfrontends

## Installation

### Core Packages

Install the core packages in your projects:

```bash
# For app shells
dotnet add package BlazorMicrofrontends.AppShell
dotnet add package BlazorMicrofrontends.Host

# For microfrontends
dotnet add package BlazorMicrofrontends.Integration
```

### CLI Tool

Install the CLI tool globally:

```bash
dotnet tool install --global BlazorMicrofrontends.CLI
```

### Project Templates

Install the project templates:

```bash
dotnet new install BlazorMicrofrontends.Templates
```

## Getting Started

### Using the CLI

Once you've installed the CLI tool, you can use it to scaffold new projects:

```bash
# Create a new app shell
blazor-mf new shell --name MyAppShell --type server

# Create a new microfrontend
blazor-mf new mf --name MyFeature --type webassembly
```

### Using the Templates Directly

If you prefer using the templates directly:

```bash
# Create a new app shell (server)
dotnet new blazormf-shell-server -n MyAppShell

# Create a new app shell (webassembly)
dotnet new blazormf-shell-wasm -n MyAppShell

# Create a new microfrontend
dotnet new blazormf-microfrontend -n MyFeature
```

### Using the Library Packages

In your app shell project:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Blazor Microfrontends services
builder.Services.AddBlazorMicrofrontends(options =>
{
    options.DiscoveryEndpoint = "/api/microfrontends";
    options.LoadRemoteMicrofrontends = true;
});

// ...
```

In your microfrontend project:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register the microfrontend with the integration service
builder.Services.AddMicrofrontendIntegration(options =>
{
    options.Name = "MyFeature";
    options.EntryComponent = typeof(MyFeatureRoot);
    options.Version = "1.0.0";
});

// ...
```

## Examples

For complete examples, refer to the sample projects:

- `BlazorMicrofrontends.Sample.Server` - A sample app shell using Blazor Server
- `BlazorMicrofrontends.Sample.WebAssembly` - A sample app shell using Blazor WebAssembly

## Documentation

For more detailed documentation, visit the [GitHub repository](https://github.com/WonderBoyHub/BlazorMicrofrontends).

## License

This project is licensed under the MIT License. 