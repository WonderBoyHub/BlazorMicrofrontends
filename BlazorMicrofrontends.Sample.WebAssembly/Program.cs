using BlazorMicrofrontends.Core;
using BlazorMicrofrontends.Host;
using BlazorMicrofrontends.Integration;
using BlazorMicrofrontends.Sample.WebAssembly;
using BlazorMicrofrontends.Sample.WebAssembly.Pages;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add microfrontend services
builder.Services.AddBlazorMicrofrontendsHost();
builder.Services.AddJavaScriptMicrofrontendIntegration();

// Register a sample Blazor component as a microfrontend
builder.Services.AddBlazorMicrofrontend<Counter>(
    moduleId: "blazor-counter",
    name: "Blazor Counter",
    version: "1.0.0",
    isWebAssembly: true);

// Register a sample React microfrontend
builder.Services.AddReactMicrofrontend(
    moduleId: "react-counter",
    name: "React Counter",
    version: "1.0.0",
    scriptUrl: "js/react-counter.js",
    elementId: "react-counter-container");

// Register a sample Vue microfrontend
builder.Services.AddVueMicrofrontend(
    moduleId: "vue-counter",
    name: "Vue Counter",
    version: "1.0.0",
    scriptUrl: "js/vue-counter.js",
    elementId: "vue-counter-container");

await builder.Build().RunAsync();
