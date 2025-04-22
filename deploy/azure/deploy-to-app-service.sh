#!/bin/bash

# Azure App Service deployment script for Blazor Microfrontends

# Ensure Azure CLI is installed
command -v az >/dev/null 2>&1 || { 
    echo >&2 "Azure CLI not found. Please install it first."; 
    echo >&2 "Visit: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"; 
    exit 1; 
}

# Check if user is logged in
if ! az account show > /dev/null 2>&1; then
    echo "Please log in to Azure first."
    az login
fi

# Configuration
RESOURCE_GROUP=${1:-"blazor-microfrontends-rg"}
APP_SERVICE_PLAN=${2:-"blazor-microfrontends-plan"}
APP_NAME=${3:-"blazor-microfrontends-app"}
LOCATION=${4:-"westus2"}
SKU=${5:-"B1"}

echo "Using the following configuration:"
echo "Resource Group: $RESOURCE_GROUP"
echo "App Service Plan: $APP_SERVICE_PLAN"
echo "App Name: $APP_NAME"
echo "Location: $LOCATION"
echo "SKU: $SKU"

# Create resource group if it doesn't exist
if ! az group show --name "$RESOURCE_GROUP" > /dev/null 2>&1; then
    echo "Creating resource group $RESOURCE_GROUP..."
    az group create --name "$RESOURCE_GROUP" --location "$LOCATION"
fi

# Create App Service Plan if it doesn't exist
if ! az appservice plan show --name "$APP_SERVICE_PLAN" --resource-group "$RESOURCE_GROUP" > /dev/null 2>&1; then
    echo "Creating App Service Plan $APP_SERVICE_PLAN..."
    az appservice plan create --name "$APP_SERVICE_PLAN" --resource-group "$RESOURCE_GROUP" --sku "$SKU" --is-linux
fi

# Create Web App if it doesn't exist
if ! az webapp show --name "$APP_NAME" --resource-group "$RESOURCE_GROUP" > /dev/null 2>&1; then
    echo "Creating Web App $APP_NAME..."
    az webapp create --name "$APP_NAME" --resource-group "$RESOURCE_GROUP" --plan "$APP_SERVICE_PLAN" --runtime "DOTNETCORE:8.0"
fi

# Build and publish the app
echo "Building and publishing the application..."
cd "$(dirname "$0")/../../" || exit
dotnet publish BlazorMicrofrontends.Sample.Server/BlazorMicrofrontends.Sample.Server.csproj -c Release -o ./publish

# Deploy to App Service
echo "Deploying to Azure App Service..."
cd publish || exit
zip -r ../deploy.zip ./*
cd ..
az webapp deployment source config-zip --resource-group "$RESOURCE_GROUP" --name "$APP_NAME" --src deploy.zip

# Configure the app
echo "Configuring app settings..."
az webapp config appsettings set --resource-group "$RESOURCE_GROUP" --name "$APP_NAME" --settings ASPNETCORE_ENVIRONMENT=Production

# Clean up
echo "Cleaning up..."
rm -rf publish
rm deploy.zip

echo "Deployment completed successfully!"
echo "You can access your application at: https://$APP_NAME.azurewebsites.net" 