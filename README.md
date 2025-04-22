# Blazor Microfrontends SDK 🚀

<p align="center">
  <img src="docs/assets/logo.png" alt="Blazor Microfrontends SDK" width="200" />
</p>

<p align="center">
  <b>The ultimate toolkit for building modular Blazor applications with microfrontends</b><br>
  <small>Integrate Blazor, React, Python, and more in a unified architecture</small>
</p>

<p align="center">
  <a href="#installation"><strong>Installation</strong></a> ·
  <a href="#quick-start"><strong>Quick Start</strong></a> ·
  <a href="#features"><strong>Features</strong></a> ·
  <a href="#cli-commands"><strong>CLI Commands</strong></a> ·
  <a href="#documentation"><strong>Documentation</strong></a> ·
  <a href="#examples"><strong>Examples</strong></a>
</p>

---

## Overview

Blazor Microfrontends SDK is a powerful toolkit for building modular web applications using microfrontend architecture with Blazor. It enables you to:

- Create app shells using Blazor Server, WebAssembly, or MAUI
- Develop independent microfrontends in Blazor, React, or Python
- Dynamically load and unload microfrontends at runtime
- Package and publish microfrontends as standalone components
- Integrate them seamlessly with any app shell
- Deploy across web, desktop, and mobile platforms

## Installation

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (for React microfrontends)
- [Python 3.8+](https://www.python.org/downloads/) (for Python microfrontends)

### Install the CLI Tool

```bash
dotnet tool install --global BlazorMicrofrontends.CLI
```

Verify the installation:

```bash
bmf --version
```

## Quick Start

### Create a New App Shell

```bash
bmf new app-shell -t EmptyServer -n MyAppShell
cd MyAppShell
```

### Create a Microfrontend

```bash
bmf new microfrontend -t Blazor -n MyCounter
```

### Add the Microfrontend to your App Shell

```bash
bmf add -p MyCounter -a MyAppShell
```

### Run the App Shell

```bash
cd MyAppShell
dotnet run
```

## Features

- **Technology Agnostic**: Support for Blazor, React, and Python microfrontends
- **Multiple Hosting Models**: Blazor Server, WebAssembly, and MAUI support
- **Cross-Platform**: Web, desktop, and mobile deployment options
- **Dynamic Loading**: Load microfrontends on-demand
- **Isolated Development**: Develop and test microfrontends independently
- **Simple Integration**: Easy-to-use CLI commands
- **Flexible Routing**: Route-based microfrontend activation
- **Packaging & Publishing**: NuGet, npm, and Python package support

## CLI Commands

The Blazor Microfrontends CLI (`bmf`) provides the following commands:

### Creating New Projects

```bash
# Create a new app shell
bmf new app-shell -t <template> -n <name> -o <output>

# Create a new microfrontend
bmf new microfrontend -t <template> -n <name> -o <output>
```

Templates for app shells:
- `EmptyServer`: Empty Blazor Server app shell
- `EmptyWebAssembly`: Empty Blazor WebAssembly app shell
- `EmptyMaui`: Empty MAUI app shell
- `SampleServer`: Sample Blazor Server app shell
- `SampleWebAssembly`: Sample Blazor WebAssembly app shell
- `SampleMaui`: Sample MAUI app shell

Templates for microfrontends:
- `Blazor`: Blazor microfrontend
- `BlazorMaui`: Blazor MAUI microfrontend
- `React`: React microfrontend (with options for JS/TS and CSS frameworks)
- `Python`: Python microfrontend (with options for Flask/Django)

#### React Microfrontend Options

```bash
# Create a React microfrontend with TypeScript and TailwindCSS
bmf new microfrontend -t React -n MyComponent --language TypeScript --css-framework TailwindCSS
```

Available options:
- `--language`: JavaScript or TypeScript (default: JavaScript)
- `--use-jsx`: Whether to use JSX/TSX syntax (default: true)
- `--css-framework`: CSS framework to use (Bootstrap, TailwindCSS, None) (default: Bootstrap)

#### Python Microfrontend Options

```bash
# Create a Python microfrontend with Django and PostgreSQL
bmf new microfrontend -t Python -n MyApi --framework Django --include-database --database-type PostgreSQL
```

Available options:
- `--framework`: Web framework to use (Flask, Django) (default: Flask)
- `--include-database`: Whether to include database integration (default: false)
- `--database-type`: Database type (SQLite, PostgreSQL) (default: SQLite)

### Managing Microfrontends

```bash
# Add a microfrontend to an app shell
bmf add -p <microfrontend-path> -a <app-shell-path>

# Remove a microfrontend from an app shell
bmf remove -n <microfrontend-name> -a <app-shell-path>

# Enable a microfrontend in an app shell
bmf enable -n <microfrontend-name> -a <app-shell-path>

# Disable a microfrontend in an app shell
bmf disable -n <microfrontend-name> -a <app-shell-path>
```

### Packaging and Publishing

```bash
# Package a microfrontend
bmf package microfrontend -d <microfrontend-dir> -o <output-dir>

# Package an app shell
bmf package appshell -d <app-shell-dir> -o <output-dir>

# Publish a microfrontend as a package
bmf publish microfrontend -p <microfrontend-dir> -r <repository-url> -k <api-key>

# Publish an app shell as a package
bmf publish app-shell -p <app-shell-dir> -r <repository-url> -k <api-key>
```

### Other Commands

```bash
# List microfrontends in an app shell
bmf list microfrontends -a <app-shell-path>

# List available packages in a repository
bmf list repository -r <repository-url>
```

## Documentation

### Core Components

The SDK consists of several packages:

- **BlazorMicrofrontends.AppShell**: Library for creating app shells
- **BlazorMicrofrontends.Microfrontend**: Library for creating microfrontends
- **BlazorMicrofrontends.React**: Integration with React microfrontends
- **BlazorMicrofrontends.Python**: Integration with Python microfrontends
- **BlazorMicrofrontends.CLI**: Command-line interface for the SDK

### Architecture

![Architecture](docs/assets/architecture.png)

The SDK follows a microfrontend architecture where:

1. **App Shell**: The container application that hosts microfrontends
2. **Microfrontends**: Independent modules that implement specific features
3. **Router**: Loads the appropriate microfrontend based on the route
4. **Registry**: Manages microfrontend metadata and loading

### Configuration

App shells are configured using `appsettings.json`:

```json
{
  "Microfrontends": {
    "Source": "Local",
    "LocalPath": "Microfrontends",
    "RemoteRegistry": "",
    "Enabled": ["MyCounter", "MyTodo"]
  }
}
```

Microfrontends are configured using `microfrontend.json`:

```json
{
  "name": "MyCounter",
  "version": "1.0.0",
  "description": "A simple counter microfrontend",
  "displayName": "Counter",
  "author": "Your Name",
  "entryPoint": "MainComponent",
  "routes": [
    {
      "path": "",
      "title": "Counter"
    },
    {
      "path": "counter",
      "title": "Counter"
    }
  ],
  "dependencies": [],
  "environments": ["web", "desktop", "mobile"]
}
```

## Examples

### Blazor Microfrontend

```csharp
@page "/{**path}"
@using BlazorMicrofrontends.Microfrontend.Interfaces
@inject IMicrofrontendService MicrofrontendService

<div class="container">
    <h2>@MicrofrontendService.GetName() Microfrontend</h2>
    
    <div class="counter">
        <p>Current count: @currentCount</p>
        <button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
    </div>
</div>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

### React Microfrontend

```jsx
import React, { useState } from 'react';
import { MicrofrontendApi } from 'blazor-microfrontends-react';

function App() {
  const [count, setCount] = useState(0);
  
  return (
    <div className="container">
      <h2>{MicrofrontendApi.getName()} Microfrontend</h2>
      
      <div className="counter">
        <p>Current count: {count}</p>
        <button 
          className="btn btn-primary" 
          onClick={() => setCount(count + 1)}
        >
          Click me
        </button>
      </div>
    </div>
  );
}

export default App;
```

## MAUI Support

For detailed instructions on integrating with MAUI, see the [MAUI Integration Guide](docs/maui-integration.md).

### Key MAUI Features

- Native device capabilities in microfrontends
- Cross-platform deployment (Android, iOS, Windows, macOS)
- Offline support
- Hardware acceleration

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for details. 