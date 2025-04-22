# Blazor Microfrontends SDK Release Checklist

Follow this checklist to release a new version of the Blazor Microfrontends SDK.

## Pre-Release Checks

- [ ] Ensure all tests are passing (`dotnet test`)
- [ ] Update version numbers in all project files:
  - [ ] `BlazorMicrofrontends.Core/BlazorMicrofrontends.Core.csproj`
  - [ ] `BlazorMicrofrontends.Host/BlazorMicrofrontends.Host.csproj`
  - [ ] `BlazorMicrofrontends.Integration/BlazorMicrofrontends.Integration.csproj`
  - [ ] `BlazorMicrofrontends.AppShell/BlazorMicrofrontends.AppShell.csproj`
  - [ ] `BlazorMicrofrontends.CLI/BlazorMicrofrontends.CLI.csproj`
  - [ ] `BlazorMicrofrontends.Templates/BlazorMicrofrontends.Templates.csproj`
- [ ] Update CHANGELOG.md with new version and changes
- [ ] Make sure README.md is up to date
- [ ] Commit all changes with message "Release vX.Y.Z"

## Manual Release

If not using the GitHub Actions workflow, follow these steps:

1. Build the NuGet packages:
   - On Linux/macOS: Run `./build-nuget.sh`
   - On Windows: Run `.\build-nuget.bat`

2. Check the generated packages in the `dist` directory

3. Push packages to NuGet.org:
   ```bash
   # Replace YOUR_API_KEY with your NuGet API key
   # Replace 1.0.0 with the actual version
   dotnet nuget push dist/BlazorMicrofrontends.Core.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   dotnet nuget push dist/BlazorMicrofrontends.Host.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   dotnet nuget push dist/BlazorMicrofrontends.Integration.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   dotnet nuget push dist/BlazorMicrofrontends.AppShell.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   dotnet nuget push dist/BlazorMicrofrontends.CLI.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   dotnet nuget push dist/BlazorMicrofrontends.Templates.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   ```

## GitHub Actions Release

To use the GitHub Actions workflow:

1. Create a GitHub release:
   - Go to the repository's Releases page
   - Click "Create a new release"
   - Set the tag version (e.g., `v1.0.0`)
   - Set the release title (e.g., "Blazor Microfrontends SDK v1.0.0")
   - Add release notes (can be copied from CHANGELOG.md)
   - Click "Publish release"

2. This will trigger the GitHub Actions workflow that will:
   - Build the projects
   - Create the NuGet packages
   - Push them to NuGet.org

Note: Before using the GitHub Actions workflow, make sure to:
1. Add the `NUGET_API_KEY` secret to your GitHub repository
2. Ensure GitHub Actions is enabled for your repository

## Post-Release

- [ ] Verify packages are available on NuGet.org
- [ ] Test installing the packages in a new project
- [ ] Test installing the CLI tool: `dotnet tool install --global BlazorMicrofrontends.CLI`
- [ ] Test installing the templates: `dotnet new install BlazorMicrofrontends.Templates`
- [ ] Announce the release on relevant platforms
- [ ] Start next development cycle by updating version numbers to next development version (e.g., 1.0.1-preview)

## Troubleshooting

If packages fail to publish:
- Check that package versions don't already exist on NuGet.org
- Verify API key permissions
- Look at GitHub Actions workflow logs for errors 