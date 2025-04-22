using BlazorMicrofrontends.Core;
using BlazorMicrofrontends.Host;
using BlazorMicrofrontends.Integration;
using BlazorMicrofrontends.Sample.Server.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add microfrontend services
builder.Services.AddBlazorMicrofrontendsHost();
builder.Services.AddJavaScriptMicrofrontendIntegration();

// Add a sample Blazor Server component as a microfrontend
builder.Services.AddBlazorMicrofrontend<Counter>(
    moduleId: "server-counter",
    name: "Server Counter",
    version: "1.0.0",
    isWebAssembly: false);

// Add a sample React microfrontend
builder.Services.AddReactMicrofrontend(
    moduleId: "react-todo",
    name: "React Todo App",
    version: "1.0.0",
    scriptUrl: "/js/react-todo.js",
    elementId: "react-todo-container",
    cssUrl: "/css/react-todo.css");

// Register a WebAssembly microfrontend (from the other project)
builder.Services.AddBlazorMicrofrontend<BlazorMicrofrontends.Sample.WebAssembly.Pages.Counter>(
    moduleId: "wasm-counter",
    name: "WebAssembly Counter",
    version: "1.0.0",
    isWebAssembly: true);

// Add Python PyScript integration example
builder.Services.AddJavaScriptMicrofrontend(
    moduleId: "python-calculator",
    name: "Python Calculator",
    version: "1.0.0",
    technology: "PyScript",
    scriptUrl: "/js/pyscript/pyscript.js",
    elementId: "python-calculator-container",
    mountFunction: "initialize",
    cssUrl: "/js/pyscript/pyscript.css");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Configure the endpoints
var endpoints = app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Manually add the WebAssembly assembly
endpoints.AddAdditionalAssemblies(typeof(BlazorMicrofrontends.Sample.WebAssembly._Imports).Assembly);

// Initialize all microfrontends on application startup
app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var lifecycle = scope.ServiceProvider.GetRequiredService<IMicrofrontendLifecycle>();
    await lifecycle.InitializeAllModulesAsync();
});

app.Run();
