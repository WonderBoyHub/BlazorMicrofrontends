using System;
using System.Threading.Tasks;
using BlazorMicrofrontends.Core;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.Integration.Tests;

public class JsMicrofrontendModuleTests
{
    private readonly Mock<IJSRuntime> _mockJsRuntime;
    
    public JsMicrofrontendModuleTests()
    {
        _mockJsRuntime = new Mock<IJSRuntime>();
    }
    
    [Fact]
    public void Constructor_SetsProperties()
    {
        // Arrange & Act
        var module = new JsMicrofrontendModule(
            "test-module",
            "Test Module",
            "1.0.0",
            "react",
            "script.js",
            _mockJsRuntime.Object,
            "container",
            "mount",
            "unmount",
            "style.css");
        
        // Assert
        Assert.Equal("test-module", module.ModuleId);
        Assert.Equal("Test Module", module.Name);
        Assert.Equal("1.0.0", module.Version);
        Assert.Equal("react", module.Technology);
        Assert.Equal("script.js", module.ScriptUrl);
        Assert.Equal("style.css", module.CssUrl);
        Assert.Equal("container", module.ElementId);
        Assert.False(module.IsInitialized);
    }
    
    [Fact]
    public async Task InitializeAsync_CallsJavaScriptObject()
    {
        // Arrange
        var module = new JsMicrofrontendModule(
            "test-module",
            "Test Module",
            "1.0.0",
            "react",
            "script.js",
            _mockJsRuntime.Object,
            "container",
            "mount",
            "unmount");
        
        _mockJsRuntime.Setup(js => js.InvokeAsync<object>(
            "import", 
            It.Is<object[]>(args => args[0].ToString() == "script.js")))
            .ReturnsAsync(new { });
        
        // Act
        await module.InitializeAsync();
        
        // Assert
        _mockJsRuntime.Verify(js => js.InvokeAsync<object>("import", It.IsAny<object[]>()), Times.Once);
        Assert.True(module.IsInitialized);
    }
    
    [Fact]
    public async Task MountAsync_CallsMountFunction()
    {
        // Arrange
        var module = new JsMicrofrontendModule(
            "test-module",
            "Test Module",
            "1.0.0",
            "react",
            "script.js",
            _mockJsRuntime.Object,
            "container",
            "mount",
            "unmount");
        
        var jsModule = new { };
        _mockJsRuntime.Setup(js => js.InvokeAsync<object>("import", It.IsAny<object[]>()))
            .ReturnsAsync(jsModule);
        
        await module.InitializeAsync();
        
        _mockJsRuntime.Setup(js => js.InvokeAsync<object>(
            It.Is<string>(s => s == "globalThis.test_module_mount"),
            It.IsAny<object[]>()))
            .ReturnsAsync(new { });
        
        // Act
        await module.MountAsync("container", new { prop1 = "value1" });
        
        // Assert
        _mockJsRuntime.Verify(js => js.InvokeAsync<object>(
            "globalThis.test_module_mount", 
            It.Is<object[]>(args => args[0].ToString() == "container")), 
            Times.Once);
    }
    
    [Fact]
    public async Task UnmountAsync_CallsUnmountFunction()
    {
        // Arrange
        var module = new JsMicrofrontendModule(
            "test-module",
            "Test Module",
            "1.0.0",
            "react",
            "script.js",
            _mockJsRuntime.Object,
            "container",
            "mount",
            "unmount");
        
        var jsModule = new { };
        _mockJsRuntime.Setup(js => js.InvokeAsync<object>("import", It.IsAny<object[]>()))
            .ReturnsAsync(jsModule);
        
        await module.InitializeAsync();
        
        _mockJsRuntime.Setup(js => js.InvokeAsync<object>(
            It.Is<string>(s => s == "globalThis.test_module_unmount"),
            It.IsAny<object[]>()))
            .ReturnsAsync(new { });
        
        // Act
        await module.UnmountAsync();
        
        // Assert
        _mockJsRuntime.Verify(js => js.InvokeAsync<object>(
            "globalThis.test_module_unmount", 
            It.IsAny<object[]>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task CleanupAsync_CallsUnmountAndClearsIsInitialized()
    {
        // Arrange
        var module = new JsMicrofrontendModule(
            "test-module",
            "Test Module",
            "1.0.0",
            "react",
            "script.js",
            _mockJsRuntime.Object,
            "container",
            "mount",
            "unmount");
        
        var jsModule = new { };
        _mockJsRuntime.Setup(js => js.InvokeAsync<object>("import", It.IsAny<object[]>()))
            .ReturnsAsync(jsModule);
        
        await module.InitializeAsync();
        
        _mockJsRuntime.Setup(js => js.InvokeAsync<object>(
            It.Is<string>(s => s == "globalThis.test_module_unmount"),
            It.IsAny<object[]>()))
            .ReturnsAsync(new { });
        
        // Act
        await module.CleanupAsync();
        
        // Assert
        _mockJsRuntime.Verify(js => js.InvokeAsync<object>(
            "globalThis.test_module_unmount", 
            It.IsAny<object[]>()), 
            Times.Once);
        Assert.False(module.IsInitialized);
    }
    
    [Fact]
    public void AddRoute_AddsRouteToCollection()
    {
        // Arrange
        var module = new JsMicrofrontendModule(
            "test-module",
            "Test Module",
            "1.0.0",
            "react",
            "script.js",
            _mockJsRuntime.Object);
        
        // Act
        module.AddRoute("test-route", "Test Route");
        
        // Assert
        Assert.Single(module.Routes);
        Assert.Equal("test-route", module.Routes[0].Path);
        Assert.Equal("Test Route", module.Routes[0].Title);
    }
} 