using System;
using System.Threading.Tasks;
using Bunit;
using BlazorMicrofrontends.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.Integration.Tests;

public class JsMicrofrontendHostTests : TestContext
{
    private readonly Mock<JsMicrofrontendModule> _mockModule;
    private readonly Mock<IJSRuntime> _mockJsRuntime;
    
    public JsMicrofrontendHostTests()
    {
        _mockJsRuntime = new Mock<IJSRuntime>();
        _mockModule = new Mock<JsMicrofrontendModule>(
            "test-module",
            "Test Module",
            "1.0.0",
            "react",
            "script.js",
            _mockJsRuntime.Object);
        
        _mockModule.Setup(m => m.ModuleId).Returns("test-module");
        _mockModule.Setup(m => m.Name).Returns("Test Module");
        _mockModule.Setup(m => m.Version).Returns("1.0.0");
        _mockModule.Setup(m => m.Technology).Returns("react");
        _mockModule.Setup(m => m.IsInitialized).Returns(true);
        _mockModule.Setup(m => m.MountAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(Task.CompletedTask);
        _mockModule.Setup(m => m.UnmountAsync())
            .Returns(Task.CompletedTask);
        
        Services.AddSingleton(_mockJsRuntime.Object);
        JSInterop.SetupModule("_content/BlazorMicrofrontends.Integration/interop.js");
    }
    
    [Fact]
    public void JsMicrofrontendHost_RendersContainer()
    {
        // Arrange & Act
        var cut = RenderComponent<JsMicrofrontendHost>(parameters => parameters
            .Add(p => p.Module, _mockModule.Object)
            .Add(p => p.ContainerId, "test-container")
            .Add(p => p.CssClass, "test-class"));
        
        // Assert
        cut.MarkupMatches("<div id=\"test-container\" class=\"js-microfrontend-container test-class\"></div>");
    }
    
    [Fact]
    public void JsMicrofrontendHost_MountsModule_WhenModuleIsSet()
    {
        // Arrange & Act
        var cut = RenderComponent<JsMicrofrontendHost>(parameters => parameters
            .Add(p => p.Module, _mockModule.Object)
            .Add(p => p.ContainerId, "test-container"));
        
        // Assert
        _mockModule.Verify(m => m.MountAsync("test-container", null), Times.Once);
    }
    
    [Fact]
    public void JsMicrofrontendHost_PassesProps_WhenProvided()
    {
        // Arrange
        var props = new { Name = "Test", Value = 42 };
        
        // Act
        var cut = RenderComponent<JsMicrofrontendHost>(parameters => parameters
            .Add(p => p.Module, _mockModule.Object)
            .Add(p => p.ContainerId, "test-container")
            .Add(p => p.Props, props));
        
        // Assert
        _mockModule.Verify(m => m.MountAsync("test-container", props), Times.Once);
    }
    
    [Fact]
    public void JsMicrofrontendHost_DisplaysLoadingTemplate_WhenModuleNotInitialized()
    {
        // Arrange
        _mockModule.Setup(m => m.IsInitialized).Returns(false);
        _mockModule.Setup(m => m.InitializeAsync()).Returns(Task.CompletedTask);
        
        var loadingTemplate = new RenderFragment(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "loading");
            builder.AddContent(2, "Loading...");
            builder.CloseElement();
        });
        
        // Act
        var cut = RenderComponent<JsMicrofrontendHost>(parameters => parameters
            .Add(p => p.Module, _mockModule.Object)
            .Add(p => p.ContainerId, "test-container")
            .Add(p => p.LoadingTemplate, loadingTemplate));
        
        // Assert
        cut.MarkupMatches("<div class=\"loading\">Loading...</div>");
    }
    
    [Fact]
    public void JsMicrofrontendHost_DisplaysErrorTemplate_WhenErrorOccurs()
    {
        // Arrange
        _mockModule.Setup(m => m.MountAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new Exception("Mount error"));
        
        var errorTemplate = new RenderFragment<string>(errorMessage => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "error");
            builder.AddContent(2, errorMessage);
            builder.CloseElement();
        });
        
        // Act
        var cut = RenderComponent<JsMicrofrontendHost>(parameters => parameters
            .Add(p => p.Module, _mockModule.Object)
            .Add(p => p.ContainerId, "test-container")
            .Add(p => p.ErrorTemplate, errorTemplate));
        
        // Assert
        cut.MarkupMatches("<div class=\"error\">Mount error</div>");
    }
    
    [Fact]
    public void JsMicrofrontendHost_UnmountsModule_WhenDisposed()
    {
        // Arrange & Act
        var cut = RenderComponent<JsMicrofrontendHost>(parameters => parameters
            .Add(p => p.Module, _mockModule.Object)
            .Add(p => p.ContainerId, "test-container"));
        
        // Simulate component disposal
        cut.Dispose();
        
        // Assert
        _mockModule.Verify(m => m.UnmountAsync(), Times.Once);
    }
    
    [Fact]
    public void JsMicrofrontendHost_InvokesOnMounted_WhenModuleIsMounted()
    {
        // Arrange
        var onMountedCalled = false;
        
        // Act
        var cut = RenderComponent<JsMicrofrontendHost>(parameters => parameters
            .Add(p => p.Module, _mockModule.Object)
            .Add(p => p.ContainerId, "test-container")
            .Add(p => p.OnMounted, EventCallback.Factory.Create(this, () => onMountedCalled = true)));
        
        // Assert
        Assert.True(onMountedCalled);
    }
    
    [Fact]
    public void JsMicrofrontendHost_UnmountsOldModule_WhenModuleChanges()
    {
        // Arrange
        var mockOldModule = new Mock<JsMicrofrontendModule>(
            "old-module",
            "Old Module",
            "1.0.0",
            "react",
            "old-script.js",
            _mockJsRuntime.Object);
        
        mockOldModule.Setup(m => m.ModuleId).Returns("old-module");
        mockOldModule.Setup(m => m.IsInitialized).Returns(true);
        mockOldModule.Setup(m => m.MountAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(Task.CompletedTask);
        mockOldModule.Setup(m => m.UnmountAsync())
            .Returns(Task.CompletedTask);
        
        // Act - First render with old module
        var cut = RenderComponent<JsMicrofrontendHost>(parameters => parameters
            .Add(p => p.Module, mockOldModule.Object)
            .Add(p => p.ContainerId, "test-container"));
        
        // Then update with new module
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Module, _mockModule.Object));
        
        // Assert
        mockOldModule.Verify(m => m.UnmountAsync(), Times.Once);
        _mockModule.Verify(m => m.MountAsync("test-container", null), Times.Once);
    }
} 