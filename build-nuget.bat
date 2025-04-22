@echo off

REM Create output directory
if not exist dist mkdir dist

REM Restore dependencies
echo Restoring dependencies...
dotnet restore

REM Build projects
echo Building projects...
dotnet build -c Release

REM Create NuGet packages
echo Creating NuGet packages...

REM Core package
echo Building BlazorMicrofrontends.Core package...
dotnet pack BlazorMicrofrontends.Core\BlazorMicrofrontends.Core.csproj -c Release -o dist

REM Host package
echo Building BlazorMicrofrontends.Host package...
dotnet pack BlazorMicrofrontends.Host\BlazorMicrofrontends.Host.csproj -c Release -o dist

REM Integration package
echo Building BlazorMicrofrontends.Integration package...
dotnet pack BlazorMicrofrontends.Integration\BlazorMicrofrontends.Integration.csproj -c Release -o dist

REM AppShell package
echo Building BlazorMicrofrontends.AppShell package...
dotnet pack BlazorMicrofrontends.AppShell\BlazorMicrofrontends.AppShell.csproj -c Release -o dist

REM CLI package
echo Building BlazorMicrofrontends.CLI package...
dotnet pack BlazorMicrofrontends.CLI\BlazorMicrofrontends.CLI.csproj -c Release -o dist

REM Templates package
echo Building BlazorMicrofrontends.Templates package...
dotnet pack BlazorMicrofrontends.Templates\BlazorMicrofrontends.Templates.csproj -c Release -o dist

echo NuGet packages created successfully in the 'dist' directory
dir dist 