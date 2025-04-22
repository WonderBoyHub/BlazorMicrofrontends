using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using BlazorMicrofrontends.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.Host.Tests;

public class MicrofrontendHostTests : TestContext
{
    private readonly Mock<IMicrofrontendLifecycle> _mockLifecycle;
    private readonly Mock<IMicrofrontendModule> _mockModule;
    
    public MicrofrontendHostTests()
    {
        _mockLifecycle = new Mock<IMicrofrontendLifecycle>();
        _mockModule = new Mock<IMicrofrontendModule>();
        
        _mockModule.Setup(m => m.ModuleId).Returns("test-module");
        _mockModule.Setup(m => m.Name).Returns("Test Module");
        _mockModule.Setup(m => m.Version).Returns("1.0.0");
        _mockModule.Setup(m => m.Technology).Returns("test");
        _mockModule.Setup(m => m.IsInitialized).Returns(true);
        
        _mockLifecycle.Setup(l => l.GetModule("test-module")).Returns(_mockModule.Object);
        
        Services.AddSingleton(_mockLifecycle.Object);
        JSInterop.SetupModule("_content/BlazorMicrofrontends.Host/interop.js");
    }
    
    [Fact]
    public void MicrofrontendHost_DisplaysLoadingTemplate_Initially()
    {
        // Arrange
        _mockModule.Setup(m => m.IsInitialized).Returns(false);
        _mockLifecycle.Setup(l => l.InitializeModuleAsync("test-module")).Returns(Task.CompletedTask);
        
        var loadingTemplate = new RenderFragment(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "loading");
            builder.AddContent(2, "Loading...");
            builder.CloseElement();
        });
        
        // Act
        var cut = RenderComponent<MicrofrontendHost>(parameters => parameters
            .Add(p => p.ModuleId, "test-module")
            .Add(p => p.LoadingTemplate, loadingTemplate));
        
        // Assert
        cut.MarkupMatches("<div class=\"microfrontend-host\" id:ignore><div class=\"loading\">Loading...</div></div>");
    }
    
    [Fact]
    public void MicrofrontendHost_DisplaysErrorTemplate_WhenModuleNotFound()
    {
        // Arrange
        _mockLifecycle.Setup(l => l.GetModule("non-existent-module")).Returns((IMicrofrontendModule)null);
        
        var errorTemplate = new RenderFragment<string>(errorMessage => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "error");
            builder.AddContent(2, errorMessage);
            builder.CloseElement();
        });
        
        // Act
        var cut = RenderComponent<MicrofrontendHost>(parameters => parameters
            .Add(p => p.ModuleId, "non-existent-module")
            .Add(p => p.ErrorTemplate, errorTemplate));
        
        // Assert
        cut.MarkupMatches("<div class=\"microfrontend-host\" id:ignore><div class=\"error\">No microfrontend module with ID 'non-existent-module' was found.</div></div>");
    }
    
    [Fact]
    public void MicrofrontendHost_DisplaysModuleContent_WhenModuleIsInitialized()
    {
        // Arrange
        _mockModule.Setup(m => m.IsInitialized).Returns(true);
        
        // Act
        var cut = RenderComponent<MicrofrontendHost>(parameters => parameters
            .Add(p => p.ModuleId, "test-module"));
        
        // Assert
        // Should render the module content
        cut.MarkupMatches("<div class=\"microfrontend-host\" id:ignore><div class=\"microfrontend-content\">Module Test Module (test)</div></div>");
    }
    
    [Fact]
    public void MicrofrontendHost_InitializesModule_WhenNotInitialized()
    {
        // Arrange
        _mockModule.Setup(m => m.IsInitialized).Returns(false);
        _mockLifecycle.Setup(l => l.InitializeModuleAsync("test-module")).Returns(Task.CompletedTask);
        
        // Act
        var cut = RenderComponent<MicrofrontendHost>(parameters => parameters
            .Add(p => p.ModuleId, "test-module"));
        
        // Assert
        _mockLifecycle.Verify(l => l.InitializeModuleAsync("test-module"), Times.Once);
    }
    
    [Fact]
    public void MicrofrontendHost_InvokesOnModuleLoaded_WhenModuleIsLoaded()
    {
        // Arrange
        var onModuleLoadedCalled = false;
        IMicrofrontendModule loadedModule = null;
        
        // Act
        var cut = RenderComponent<MicrofrontendHost>(parameters => parameters
            .Add(p => p.ModuleId, "test-module")
            .Add(p => p.OnModuleLoaded, EventCallback.Factory.Create<IMicrofrontendModule>(this, module => 
            {
                onModuleLoadedCalled = true;
                loadedModule = module;
            })));
        
        // Assert
        Assert.True(onModuleLoadedCalled);
        Assert.Equal("test-module", loadedModule.ModuleId);
    }
    
    [Fact]
    public void MicrofrontendHost_DisplaysChildContent_WhenNoModuleIdProvided()
    {
        // Arrange
        var childContent = new RenderFragment(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "child-content");
            builder.AddContent(2, "Child Content");
            builder.CloseElement();
        });
        
        // Act
        var cut = RenderComponent<MicrofrontendHost>(parameters => parameters
            .Add(p => p.ChildContent, childContent));
        
        // Assert
        cut.MarkupMatches("<div class=\"microfrontend-host\" id:ignore><div class=\"child-content\">Child Content</div></div>");
    }
    
    [Fact]
    public void MicrofrontendHost_AppliesHostCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<MicrofrontendHost>(parameters => parameters
            .Add(p => p.ModuleId, "test-module")
            .Add(p => p.HostCssClass, "custom-host-class"));
        
        // Assert
        cut.MarkupMatches("<div class=\"microfrontend-host custom-host-class\" id:ignore><div class=\"microfrontend-content\">Module Test Module (test)</div></div>");
    }
} 