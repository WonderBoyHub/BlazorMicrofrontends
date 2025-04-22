# Blazor Microfrontends Templates

This package contains templates for creating Blazor Microfrontends app shells and microfrontends.

## Installation

```bash
dotnet new install BlazorMicrofrontends.Templates
```

## Templates

### App Shell Templates

- **Empty Server App Shell**: `blazormfe-server-empty`
- **Sample Server App Shell**: `blazormfe-server-sample`
- **Empty WebAssembly App Shell**: `blazormfe-wasm-empty`
- **Sample WebAssembly App Shell**: `blazormfe-wasm-sample`
- **Empty MAUI App Shell**: `blazormfe-maui-empty`
- **Sample MAUI App Shell**: `blazormfe-maui-sample`

### Microfrontend Templates

- **Blazor Microfrontend**: `blazormfe-blazor`
- **Blazor MAUI Microfrontend**: `blazormfe-maui`
- **React Microfrontend**: `blazormfe-react`
- **Python Microfrontend**: `blazormfe-python`
- **Custom Microfrontend**: `blazormfe-custom`

## Usage

### Create an App Shell

```bash
# Create a Blazor Server app shell
dotnet new blazormfe-server-empty -n MyAppShell

# Create a MAUI app shell
dotnet new blazormfe-maui-empty -n MyMauiAppShell
```

### Create a Microfrontend

```bash
# Create a Blazor microfrontend
dotnet new blazormfe-blazor -n MyCounter

# Create a React microfrontend with TypeScript
dotnet new blazormfe-react -n MyReactComponent --Language TypeScript --css-framework Bootstrap

# Create a Python microfrontend with Django
dotnet new blazormfe-python -n MyPythonComponent --Framework Django --include-database true
```

### Template Options

#### React Microfrontend Options

- `--Language`: JavaScript or TypeScript (default: JavaScript)
- `--use-jsx`: Whether to use JSX/TSX syntax (default: true)
- `--css-framework`: CSS framework to use (Bootstrap, TailwindCSS, None) (default: Bootstrap)

#### Python Microfrontend Options

- `--Framework`: Web framework to use (Flask, Django) (default: Flask)
- `--include-database`: Whether to include database integration (default: false)
- `--database-type`: Database type (SQLite, PostgreSQL) (default: SQLite)

## Using with CLI

Once you have the Blazor Microfrontends CLI installed, you can use these templates directly with additional options:

```bash
# Create an app shell
bmf new app-shell -t EmptyServer -n MyAppShell

# Create a React microfrontend with TypeScript
bmf new microfrontend -t React -n MyCounter --language TypeScript --css-framework TailwindCSS

# Create a Python microfrontend with Django and PostgreSQL database
bmf new microfrontend -t Python -n MyPythonApi --framework Django --include-database --database-type PostgreSQL
``` 