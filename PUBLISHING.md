# Publishing Blazor Microfrontends SDK to NuGet.org

This document outlines the steps to publish the Blazor Microfrontends SDK packages to NuGet.org.

## Prerequisites

1. A NuGet.org account
2. An API key from NuGet.org with push permissions
3. The .NET SDK installed

## Building the Packages

### On macOS/Linux

```bash
# Make the script executable (if not already)
chmod +x build-nuget.sh

# Run the build script
./build-nuget.sh
```

### On Windows

```powershell
# Run the build batch file
.\build-nuget.bat
```

This will create the NuGet packages in the `dist` directory.

## Publishing to NuGet.org

Once you have built the packages, you can publish them to NuGet.org using the following steps:

1. Get your API key from NuGet.org:
   - Log in to your account on [NuGet.org](https://www.nuget.org/)
   - Go to your account settings
   - Copy your API key

2. Push each package to NuGet.org:

```bash
# Core package
dotnet nuget push dist/BlazorMicrofrontends.Core.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# Host package
dotnet nuget push dist/BlazorMicrofrontends.Host.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# Integration package
dotnet nuget push dist/BlazorMicrofrontends.Integration.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# AppShell package
dotnet nuget push dist/BlazorMicrofrontends.AppShell.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# CLI package
dotnet nuget push dist/BlazorMicrofrontends.CLI.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# Templates package
dotnet nuget push dist/BlazorMicrofrontends.Templates.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

Replace `YOUR_API_KEY` with your actual NuGet.org API key.

## Tool Installation (CLI)

After publishing, users can install the CLI tool globally using:

```bash
dotnet tool install --global BlazorMicrofrontends.CLI
```

## Template Installation

After publishing, users can install the project templates using:

```bash
dotnet new install BlazorMicrofrontends.Templates
```

## Verifying the Published Packages

After publishing, you can verify that your packages are available on NuGet.org by searching for them:

[https://www.nuget.org/packages?q=BlazorMicrofrontends](https://www.nuget.org/packages?q=BlazorMicrofrontends)

It may take a few minutes for the packages to appear in the search results.

## Updating Package Versions

When releasing a new version:

1. Update the `Version` property in all `.csproj` files to the new version number
2. Build the packages again using the build script
3. Publish the new packages to NuGet.org with the updated version numbers

## Troubleshooting

If you encounter issues when pushing packages to NuGet.org:

- Ensure your API key is valid and has not expired
- Check that you have push permissions for the package ID
- Verify that the package version doesn't already exist on NuGet.org
- Check the NuGet.org status page for any service disruptions 