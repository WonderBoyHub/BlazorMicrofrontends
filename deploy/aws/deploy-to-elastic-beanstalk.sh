#!/bin/bash

# AWS Elastic Beanstalk deployment script for Blazor Microfrontends

# Ensure AWS CLI is installed
command -v aws >/dev/null 2>&1 || { 
    echo >&2 "AWS CLI not found. Please install it first."; 
    echo >&2 "Visit: https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html"; 
    exit 1; 
}

# Check if EB CLI is installed
command -v eb >/dev/null 2>&1 || { 
    echo >&2 "Elastic Beanstalk CLI not found. Please install it first."; 
    echo >&2 "Visit: https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/eb-cli3-install.html"; 
    exit 1; 
}

# Configuration
APP_NAME=${1:-"blazor-microfrontends"}
ENVIRONMENT_NAME=${2:-"production"}
REGION=${3:-"us-west-2"}
PLATFORM=${4:-".NET Core on Linux"}
INSTANCE_TYPE=${5:-"t3.small"}

echo "Using the following configuration:"
echo "Application Name: $APP_NAME"
echo "Environment Name: $ENVIRONMENT_NAME"
echo "Region: $REGION"
echo "Platform: $PLATFORM"
echo "Instance Type: $INSTANCE_TYPE"

# Get the script directory and go to the project root
cd "$(dirname "$0")/../../" || exit

# Build and publish the app
echo "Building and publishing the application..."
dotnet publish BlazorMicrofrontends.Sample.Server/BlazorMicrofrontends.Sample.Server.csproj -c Release -o ./publish

# Create the deployment package
echo "Creating deployment package..."
cd publish || exit
zip -r ../deploy.zip ./*
cd ..

# Create EB configuration files
mkdir -p .ebextensions
cat > .ebextensions/01_setup.config << EOF
option_settings:
  aws:elasticbeanstalk:container:dotnet:
    AppType: Web
  aws:elasticbeanstalk:application:environment:
    ASPNETCORE_ENVIRONMENT: Production
  aws:elasticbeanstalk:environment:
    InstanceType: $INSTANCE_TYPE
EOF

# Initialize EB application if needed
if ! eb list | grep -q "$APP_NAME"; then
    echo "Initializing Elastic Beanstalk application..."
    eb init "$APP_NAME" --region "$REGION" --platform "$PLATFORM"
fi

# Create or update the environment
if ! eb list | grep -q "$ENVIRONMENT_NAME"; then
    echo "Creating Elastic Beanstalk environment..."
    eb create "$ENVIRONMENT_NAME" --cname "$APP_NAME-$ENVIRONMENT_NAME" --sample
else
    echo "Updating existing environment..."
fi

# Deploy the app
echo "Deploying to Elastic Beanstalk..."
eb deploy "$ENVIRONMENT_NAME" --label "v1.0.0-$(date +%Y%m%d%H%M%S)" --staged

# Clean up
echo "Cleaning up..."
rm -rf publish
rm deploy.zip

echo "Deployment completed successfully!"
echo "You can access your application at: http://$APP_NAME-$ENVIRONMENT_NAME.$REGION.elasticbeanstalk.com" 