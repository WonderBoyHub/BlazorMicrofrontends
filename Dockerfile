FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BlazorMicrofrontends.Sample.Server/BlazorMicrofrontends.Sample.Server.csproj", "BlazorMicrofrontends.Sample.Server/"]
COPY ["BlazorMicrofrontends.Sample.WebAssembly/BlazorMicrofrontends.Sample.WebAssembly.csproj", "BlazorMicrofrontends.Sample.WebAssembly/"]
COPY ["BlazorMicrofrontends.Core/BlazorMicrofrontends.Core.csproj", "BlazorMicrofrontends.Core/"]
COPY ["BlazorMicrofrontends.Host/BlazorMicrofrontends.Host.csproj", "BlazorMicrofrontends.Host/"]
COPY ["BlazorMicrofrontends.Integration/BlazorMicrofrontends.Integration.csproj", "BlazorMicrofrontends.Integration/"]
COPY ["BlazorMicrofrontends.AppShell/BlazorMicrofrontends.AppShell.csproj", "BlazorMicrofrontends.AppShell/"]

RUN dotnet restore "BlazorMicrofrontends.Sample.Server/BlazorMicrofrontends.Sample.Server.csproj"

COPY . .
WORKDIR "/src/BlazorMicrofrontends.Sample.Server"
RUN dotnet build "BlazorMicrofrontends.Sample.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorMicrofrontends.Sample.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorMicrofrontends.Sample.Server.dll"] 