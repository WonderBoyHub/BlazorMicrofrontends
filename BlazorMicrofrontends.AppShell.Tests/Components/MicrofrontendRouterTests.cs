using System.Collections.Generic;
using Bunit;
using BlazorMicrofrontends.AppShell.Components;
using BlazorMicrofrontends.AppShell.Services;
using BlazorMicrofrontends.Core;
using BlazorMicrofrontends.Host;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.AppShell.Tests.Components;

public class MicrofrontendRouterTests : TestContext
{
    private readonly Mock<IMicrofrontendRegistry> _mockRegistry;
    private readonly Mock<NavigationManager> _mockNavigationManager;
    
    public MicrofrontendRouterTests()
    {
        _mockRegistry = new Mock<IMicrofrontendRegistry>();
        _mockNavigationManager = new Mock<NavigationManager>();
        
        Services.AddSingleton(_mockRegistry.Object);
        Services.AddSingleton(_mockNavigationManager.Object);
    }
    
    [Fact]
    public void MicrofrontendRouter_DisplaysNothing_WhenNoPathIsProvided()
    {
        // Arrange
        _mockNavigationManager.SetupGet(n => n.Uri).Returns("https://example.com/");
        _mockNavigationManager.Setup(n => n.ToBaseRelativePath(It.IsAny<string>())).Returns(string.Empty);
        
        // Act
        var cut = RenderComponent<MicrofrontendRouter>();
        
        // Assert
        cut.MarkupMatches(string.Empty);
    }
    
    [Fact]
    public void MicrofrontendRouter_DisplaysNotFoundMessage_WhenPathDoesNotMatchAnyMicrofrontend()
    {
        // Arrange
        _mockNavigationManager.SetupGet(n => n.Uri).Returns("https://example.com/unknown-path");
        _mockNavigationManager.Setup(n => n.ToBaseRelativePath(It.IsAny<string>())).Returns("unknown-path");
        _mockRegistry.Setup(r => r.GetMicrofrontends()).Returns(new List<IMicrofrontendModule>());
        
        // Act
        var cut = RenderComponent<MicrofrontendRouter>();
        
        // Assert
        cut.MarkupMatches("<div class=\"not-found\">The requested microfrontend route was not found.</div>");
    }
    
    [Fact]
    public void MicrofrontendRouter_RendersMicrofrontendHost_WhenPathMatchesMicrofrontendRoute()
    {
        // Arrange
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");
        mockModule.Setup(m => m.Routes).Returns(new List<(string Path, string Title)> { ("test-route", "Test Route") });
        
        _mockNavigationManager.SetupGet(n => n.Uri).Returns("https://example.com/test-route");
        _mockNavigationManager.Setup(n => n.ToBaseRelativePath(It.IsAny<string>())).Returns("test-route");
        _mockRegistry.Setup(r => r.GetMicrofrontends()).Returns(new List<IMicrofrontendModule> { mockModule.Object });
        
        // Mock the MicrofrontendHost component
        var mockHostComponent = new Mock<IComponent>();
        JSInterop.SetupModule("_content/BlazorMicrofrontends.Host/interop.js");
        
        // Act
        var cut = RenderComponent<MicrofrontendRouter>();
        
        // Assert - We're looking for a component with "moduleid" attribute matching our test module
        cut.FindComponent<MicrofrontendHost>();
        Assert.Contains("test-module", cut.Markup);
    }
    
    [Fact]
    public void MicrofrontendRouter_UpdatesContent_WhenNavigationOccurs()
    {
        // Arrange
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");
        mockModule.Setup(m => m.Routes).Returns(new List<(string Path, string Title)> { ("test-route", "Test Route") });
        
        _mockNavigationManager.SetupGet(n => n.Uri).Returns("https://example.com/");
        _mockNavigationManager.Setup(n => n.ToBaseRelativePath(It.IsAny<string>())).Returns(string.Empty);
        _mockRegistry.Setup(r => r.GetMicrofrontends()).Returns(new List<IMicrofrontendModule> { mockModule.Object });
        
        // Act
        var cut = RenderComponent<MicrofrontendRouter>();
        
        // Initial render should be empty
        cut.MarkupMatches(string.Empty);
        
        // Simulate navigation to a microfrontend route
        _mockNavigationManager.SetupGet(n => n.Uri).Returns("https://example.com/test-route");
        _mockNavigationManager.Setup(n => n.ToBaseRelativePath(It.IsAny<string>())).Returns("test-route");
        
        // Trigger the LocationChanged event
        var args = new LocationChangedEventArgs("https://example.com/test-route", false);
        _mockNavigationManager.Raise(n => n.LocationChanged += null, args);
        
        // Assert
        cut.FindComponent<MicrofrontendHost>();
        Assert.Contains("test-module", cut.Markup);
    }
} 