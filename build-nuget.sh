#!/bin/bash

# Create output directory
mkdir -p dist

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Build projects
echo "Building projects..."
dotnet build -c Release

# Create NuGet packages
echo "Creating NuGet packages..."

# Core package
echo "Building BlazorMicrofrontends.Core package..."
dotnet pack BlazorMicrofrontends.Core/BlazorMicrofrontends.Core.csproj -c Release -o dist

# Host package
echo "Building BlazorMicrofrontends.Host package..."
dotnet pack BlazorMicrofrontends.Host/BlazorMicrofrontends.Host.csproj -c Release -o dist

# Integration package
echo "Building BlazorMicrofrontends.Integration package..."
dotnet pack BlazorMicrofrontends.Integration/BlazorMicrofrontends.Integration.csproj -c Release -o dist

# AppShell package
echo "Building BlazorMicrofrontends.AppShell package..."
dotnet pack BlazorMicrofrontends.AppShell/BlazorMicrofrontends.AppShell.csproj -c Release -o dist

echo "NuGet packages created successfully in the 'dist' directory"
ls -la dist 