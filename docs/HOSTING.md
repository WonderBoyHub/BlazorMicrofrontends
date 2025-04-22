# Hosting Blazor Microfrontends SDK

This guide provides instructions for hosting and deploying applications built with the Blazor Microfrontends SDK in various environments.

## Table of Contents

- [Docker](#docker)
- [Microsoft Azure](#microsoft-azure)
- [Amazon Web Services (AWS)](#amazon-web-services-aws)
- [GitHub Pages](#github-pages)
- [Self-Hosted](#self-hosted)
- [Troubleshooting](#troubleshooting)

## Docker

The SDK includes a Dockerfile and docker-compose.yml configuration for easy containerization of your application.

### Running the Sample Application

1. Navigate to the project root directory
2. Build and run using Docker Compose:

```bash
docker-compose up --build
```

3. Access the application at http://localhost:8080

### Building Your Own Docker Image

If you're building your own Blazor Microfrontends application:

1. Copy the Dockerfile and docker-compose.yml from the SDK to your project
2. Modify the paths in the Dockerfile to match your project structure
3. Build and run your container:

```bash
docker build -t your-app-name .
docker run -p 8080:8080 your-app-name
```

## Microsoft Azure

### Azure App Service Deployment

The SDK includes a deployment script for Azure App Service at `deploy/azure/deploy-to-app-service.sh`.

To deploy:

1. Install the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
2. Log in to Azure:

```bash
az login
```

3. Run the deployment script:

```bash
chmod +x deploy/azure/deploy-to-app-service.sh
./deploy/azure/deploy-to-app-service.sh <resource-group> <app-service-plan> <app-name> <location> <sku>
```

Replace the parameters with your own values or omit them to use the defaults.

### Azure Container Apps

To deploy as a container to Azure Container Apps:

1. Build your Docker image:

```bash
docker build -t your-app-name .
```

2. Tag and push to Azure Container Registry:

```bash
az acr login --name YourRegistryName
docker tag your-app-name YourRegistryName.azurecr.io/your-app-name:latest
docker push YourRegistryName.azurecr.io/your-app-name:latest
```

3. Create a Container App:

```bash
az containerapp create \
  --name your-app-name \
  --resource-group your-resource-group \
  --environment your-container-apps-environment \
  --image YourRegistryName.azurecr.io/your-app-name:latest \
  --target-port 8080 \
  --ingress external
```

## Amazon Web Services (AWS)

### Elastic Beanstalk Deployment

The SDK includes a deployment script for AWS Elastic Beanstalk at `deploy/aws/deploy-to-elastic-beanstalk.sh`.

To deploy:

1. Install the [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html) and [EB CLI](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/eb-cli3-install.html)
2. Configure your AWS credentials:

```bash
aws configure
```

3. Run the deployment script:

```bash
chmod +x deploy/aws/deploy-to-elastic-beanstalk.sh
./deploy/aws/deploy-to-elastic-beanstalk.sh <app-name> <environment-name> <region> <platform> <instance-type>
```

Replace the parameters with your own values or omit them to use the defaults.

### ECS/Fargate

To deploy using Amazon ECS with Fargate:

1. Push your Docker image to Amazon ECR:

```bash
aws ecr create-repository --repository-name your-app-name
aws ecr get-login-password --region your-region | docker login --username AWS --password-stdin your-account-id.dkr.ecr.your-region.amazonaws.com
docker tag your-app-name:latest your-account-id.dkr.ecr.your-region.amazonaws.com/your-app-name:latest
docker push your-account-id.dkr.ecr.your-region.amazonaws.com/your-app-name:latest
```

2. Create a task definition, cluster, and service using the AWS Management Console or AWS CLI.

## GitHub Pages

Blazor WebAssembly applications can be hosted on GitHub Pages. Here's how to set it up:

1. Create a GitHub Actions workflow file in `.github/workflows/github-pages.yml`:

```yaml
name: Deploy to GitHub Pages

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'

    - name: Publish WebAssembly App
      run: dotnet publish YourWebAssemblyApp.csproj -c Release -o release --nologo

    - name: Update base href
      run: sed -i 's/<base href="\/" \/>/<base href="\/your-repo-name\/" \/>/g' release/wwwroot/index.html

    - name: Add .nojekyll file
      run: touch release/wwwroot/.nojekyll

    - name: GitHub Pages
      uses: JamesIves/github-pages-deploy-action@v4
      with:
        branch: gh-pages
        folder: release/wwwroot
```

2. Replace `YourWebAssemblyApp.csproj` with your WebAssembly project name
3. Replace `your-repo-name` with your GitHub repository name
4. Push to GitHub and the action will deploy to GitHub Pages

## Self-Hosted

To self-host your Blazor Microfrontends application:

1. Publish your application:

```bash
dotnet publish YourApp.csproj -c Release -o publish
```

2. Copy the published files to your web server
3. For IIS, install the [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-8.0.0-windows-hosting-bundle-installer)
4. Configure your web server to serve the application

## Troubleshooting

### Common Issues

#### CORS Errors with Microfrontends

If you experience CORS issues with remote microfrontends:

1. Ensure your app shell has proper CORS configuration:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MicrofrontendPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// In Configure method:
app.UseCors("MicrofrontendPolicy");
```

#### WebAssembly Module Loading Issues

If WebAssembly microfrontends fail to load:

1. Check browser console for errors
2. Ensure the `BlazorWebAssemblyResourceProvider` service is properly configured
3. Verify that the microfrontend's assembly names are correctly registered

#### Docker Networking Issues

If services can't communicate in Docker:

1. Ensure all containers are on the same network
2. Use service names instead of localhost for inter-container communication
3. Check Docker logs: `docker-compose logs -f`

For more troubleshooting help, refer to the [GitHub Issues](https://github.com/your-username/BlazorMicrofrontends/issues) page or create a new issue. 