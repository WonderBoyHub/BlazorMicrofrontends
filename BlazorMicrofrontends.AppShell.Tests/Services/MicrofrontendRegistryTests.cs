using System;
using System.Linq;
using BlazorMicrofrontends.AppShell.Services;
using BlazorMicrofrontends.Core;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.AppShell.Tests.Services;

public class MicrofrontendRegistryTests
{
    [Fact]
    public void GetMicrofrontends_ReturnsEmptyCollection_WhenNoMicrofrontendsRegistered()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();

        // Act
        var microfrontends = registry.GetMicrofrontends();

        // Assert
        Assert.Empty(microfrontends);
    }

    [Fact]
    public void RegisterMicrofrontend_AddsModuleToRegistry()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");

        // Act
        registry.RegisterMicrofrontend(mockModule.Object);
        var microfrontends = registry.GetMicrofrontends();

        // Assert
        Assert.Single(microfrontends);
        Assert.Equal("test-module", microfrontends.First().ModuleId);
    }

    [Fact]
    public void GetMicrofrontend_ReturnsNull_WhenModuleNotFound()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();

        // Act
        var module = registry.GetMicrofrontend("non-existent-module");

        // Assert
        Assert.Null(module);
    }

    [Fact]
    public void GetMicrofrontend_ReturnsModule_WhenModuleExists()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");
        registry.RegisterMicrofrontend(mockModule.Object);

        // Act
        var module = registry.GetMicrofrontend("test-module");

        // Assert
        Assert.NotNull(module);
        Assert.Equal("test-module", module.ModuleId);
    }

    [Fact]
    public void RegisterMicrofrontend_ThrowsArgumentNullException_WhenModuleIsNull()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => registry.RegisterMicrofrontend(null));
    }

    [Fact]
    public void RegisterMicrofrontend_ThrowsArgumentException_WhenModuleIdIsEmpty()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns(string.Empty);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => registry.RegisterMicrofrontend(mockModule.Object));
    }

    [Fact]
    public void RegisterMicrofrontend_ThrowsInvalidOperationException_WhenModuleAlreadyRegistered()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();
        var mockModule1 = new Mock<IMicrofrontendModule>();
        mockModule1.Setup(m => m.ModuleId).Returns("test-module");
        var mockModule2 = new Mock<IMicrofrontendModule>();
        mockModule2.Setup(m => m.ModuleId).Returns("test-module");

        registry.RegisterMicrofrontend(mockModule1.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => registry.RegisterMicrofrontend(mockModule2.Object));
    }

    [Fact]
    public void UnregisterMicrofrontend_RemovesModule_WhenModuleExists()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");
        registry.RegisterMicrofrontend(mockModule.Object);

        // Act
        registry.UnregisterMicrofrontend("test-module");
        var microfrontends = registry.GetMicrofrontends();

        // Assert
        Assert.Empty(microfrontends);
    }

    [Fact]
    public void UnregisterMicrofrontend_DoesNotThrow_WhenModuleDoesNotExist()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();

        // Act & Assert
        var exception = Record.Exception(() => registry.UnregisterMicrofrontend("non-existent-module"));
        Assert.Null(exception);
    }

    [Fact]
    public void UnregisterMicrofrontend_ThrowsArgumentException_WhenModuleIdIsEmpty()
    {
        // Arrange
        var registry = new BlazorMicrofrontends.AppShell.Services.MicrofrontendRegistry();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => registry.UnregisterMicrofrontend(string.Empty));
    }
} 