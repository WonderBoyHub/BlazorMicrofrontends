using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using BlazorMicrofrontends.AppShell.Components;
using BlazorMicrofrontends.AppShell.Services;
using BlazorMicrofrontends.Core;
using BlazorMicrofrontends.Integration;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.AppShell.Tests.Components;

public class MicrofrontendScriptsLoaderTests : TestContext
{
    private readonly Mock<IMicrofrontendRegistry> _mockRegistry;
    private readonly Mock<IJSRuntime> _mockJsRuntime;
    
    public MicrofrontendScriptsLoaderTests()
    {
        _mockRegistry = new Mock<IMicrofrontendRegistry>();
        _mockJsRuntime = new Mock<IJSRuntime>();
        
        Services.AddSingleton(_mockRegistry.Object);
        Services.AddSingleton(_mockJsRuntime.Object);
    }
    
    [Fact]
    public void MicrofrontendScriptsLoader_RendersContainer()
    {
        // Arrange
        _mockRegistry.Setup(r => r.GetMicrofrontends()).Returns(new List<IMicrofrontendModule>());
        
        // Act
        var cut = RenderComponent<MicrofrontendScriptsLoader>();
        
        // Assert
        cut.MarkupMatches("<div id=\"microfrontend-scripts-container\" style=\"display: none;\"></div>");
    }
    
    [Fact]
    public void MicrofrontendScriptsLoader_LoadsJavaScriptAndCssForJsMicrofrontend()
    {
        // Arrange
        var mockJsModule = new Mock<JsMicrofrontendModule>("test-module", "Test Module", "1.0.0", "react", "script.js", _mockJsRuntime.Object);
        mockJsModule.SetupGet(m => m.ModuleId).Returns("test-module");
        mockJsModule.SetupGet(m => m.ScriptUrl).Returns("https://example.com/script.js");
        mockJsModule.SetupGet(m => m.CssUrl).Returns("https://example.com/styles.css");
        
        var modules = new List<IMicrofrontendModule> { mockJsModule.Object };
        _mockRegistry.Setup(r => r.GetMicrofrontends()).Returns(modules);
        
        _mockJsRuntime.Setup(js => js.InvokeVoidAsync(It.IsAny<string>(), It.IsAny<object[]>()))
            .Returns(ValueTask.CompletedTask);
        
        // Act
        var cut = RenderComponent<MicrofrontendScriptsLoader>();
        
        // Assert
        _mockJsRuntime.Verify(js => js.InvokeVoidAsync("eval", It.Is<object[]>(args => 
            ((string)args[0]).Contains("script.src = 'https://example.com/script.js'"))), Times.Once);
        
        _mockJsRuntime.Verify(js => js.InvokeVoidAsync("eval", It.Is<object[]>(args => 
            ((string)args[0]).Contains("link.href = 'https://example.com/styles.css'"))), Times.Once);
    }
    
    [Fact]
    public void MicrofrontendScriptsLoader_DoesNotLoadResourcesForNonJsMicrofrontends()
    {
        // Arrange
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.SetupGet(m => m.ModuleId).Returns("blazor-module");
        
        var modules = new List<IMicrofrontendModule> { mockModule.Object };
        _mockRegistry.Setup(r => r.GetMicrofrontends()).Returns(modules);
        
        // Act
        var cut = RenderComponent<MicrofrontendScriptsLoader>();
        
        // Assert
        _mockJsRuntime.Verify(js => js.InvokeVoidAsync(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
    }
    
    [Fact]
    public void MicrofrontendScriptsLoader_LoadsOnlyScriptWhenCssUrlIsEmpty()
    {
        // Arrange
        var mockJsModule = new Mock<JsMicrofrontendModule>("test-module", "Test Module", "1.0.0", "react", "script.js", _mockJsRuntime.Object);
        mockJsModule.SetupGet(m => m.ModuleId).Returns("test-module");
        mockJsModule.SetupGet(m => m.ScriptUrl).Returns("https://example.com/script.js");
        mockJsModule.SetupGet(m => m.CssUrl).Returns(string.Empty);
        
        var modules = new List<IMicrofrontendModule> { mockJsModule.Object };
        _mockRegistry.Setup(r => r.GetMicrofrontends()).Returns(modules);
        
        _mockJsRuntime.Setup(js => js.InvokeVoidAsync(It.IsAny<string>(), It.IsAny<object[]>()))
            .Returns(ValueTask.CompletedTask);
        
        // Act
        var cut = RenderComponent<MicrofrontendScriptsLoader>();
        
        // Assert
        _mockJsRuntime.Verify(js => js.InvokeVoidAsync("eval", It.Is<object[]>(args => 
            ((string)args[0]).Contains("script.src = 'https://example.com/script.js'"))), Times.Once);
        
        _mockJsRuntime.Verify(js => js.InvokeVoidAsync("eval", It.Is<object[]>(args => 
            ((string)args[0]).Contains("link.href"))), Times.Never);
    }
    
    [Fact]
    public void MicrofrontendScriptsLoader_SkipsLoadingAlreadyLoadedScripts()
    {
        // Arrange
        var mockJsModule1 = new Mock<JsMicrofrontendModule>("module1", "Module 1", "1.0.0", "react", "script.js", _mockJsRuntime.Object);
        mockJsModule1.SetupGet(m => m.ModuleId).Returns("module1");
        mockJsModule1.SetupGet(m => m.ScriptUrl).Returns("https://example.com/script.js");
        
        var mockJsModule2 = new Mock<JsMicrofrontendModule>("module2", "Module 2", "1.0.0", "react", "script.js", _mockJsRuntime.Object);
        mockJsModule2.SetupGet(m => m.ModuleId).Returns("module2");
        mockJsModule2.SetupGet(m => m.ScriptUrl).Returns("https://example.com/script.js"); // Same script URL
        
        var modules = new List<IMicrofrontendModule> { mockJsModule1.Object, mockJsModule2.Object };
        _mockRegistry.Setup(r => r.GetMicrofrontends()).Returns(modules);
        
        _mockJsRuntime.Setup(js => js.InvokeVoidAsync(It.IsAny<string>(), It.IsAny<object[]>()))
            .Returns(ValueTask.CompletedTask);
        
        // Act
        var cut = RenderComponent<MicrofrontendScriptsLoader>();
        
        // Assert - Script should only be loaded once despite being referenced by two modules
        _mockJsRuntime.Verify(js => js.InvokeVoidAsync("eval", It.Is<object[]>(args => 
            ((string)args[0]).Contains("script.src = 'https://example.com/script.js'"))), Times.Once);
    }
} 