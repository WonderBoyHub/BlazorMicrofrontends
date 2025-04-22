using System;
using System.Threading.Tasks;
using BlazorMicrofrontends.Core;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.Core.Tests;

public class MicrofrontendLifecycleTests
{
    [Fact]
    public void RegisterModule_AddsModuleToRegistry()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");

        // Act
        lifecycle.RegisterModule(mockModule.Object);

        // Assert
        var module = lifecycle.GetModule("test-module");
        Assert.NotNull(module);
        Assert.Equal("test-module", module.ModuleId);
    }

    [Fact]
    public void RegisterModule_ThrowsArgumentNullException_WhenModuleIsNull()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => lifecycle.RegisterModule(null));
    }

    [Fact]
    public void GetModule_ReturnsNull_WhenModuleNotFound()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();

        // Act
        var module = lifecycle.GetModule("non-existent-module");

        // Assert
        Assert.Null(module);
    }

    [Fact]
    public async Task InitializeModuleAsync_CallsModuleInitialize()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");
        mockModule.Setup(m => m.InitializeAsync()).Returns(Task.CompletedTask);
        mockModule.SetupGet(m => m.IsInitialized).Returns(false);
        lifecycle.RegisterModule(mockModule.Object);

        // Act
        await lifecycle.InitializeModuleAsync("test-module");

        // Assert
        mockModule.Verify(m => m.InitializeAsync(), Times.Once);
    }

    [Fact]
    public async Task InitializeModuleAsync_DoesNotCallInitialize_WhenModuleAlreadyInitialized()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");
        mockModule.SetupGet(m => m.IsInitialized).Returns(true);
        lifecycle.RegisterModule(mockModule.Object);

        // Act
        await lifecycle.InitializeModuleAsync("test-module");

        // Assert
        mockModule.Verify(m => m.InitializeAsync(), Times.Never);
    }

    [Fact]
    public async Task InitializeModuleAsync_ThrowsInvalidOperationException_WhenModuleNotFound()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            lifecycle.InitializeModuleAsync("non-existent-module"));
    }

    [Fact]
    public async Task InitializeAllModulesAsync_InitializesAllModules()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();
        
        var mockModule1 = new Mock<IMicrofrontendModule>();
        mockModule1.Setup(m => m.ModuleId).Returns("module1");
        mockModule1.Setup(m => m.InitializeAsync()).Returns(Task.CompletedTask);
        mockModule1.SetupGet(m => m.IsInitialized).Returns(false);
        
        var mockModule2 = new Mock<IMicrofrontendModule>();
        mockModule2.Setup(m => m.ModuleId).Returns("module2");
        mockModule2.Setup(m => m.InitializeAsync()).Returns(Task.CompletedTask);
        mockModule2.SetupGet(m => m.IsInitialized).Returns(false);
        
        lifecycle.RegisterModule(mockModule1.Object);
        lifecycle.RegisterModule(mockModule2.Object);

        // Act
        await lifecycle.InitializeAllModulesAsync();

        // Assert
        mockModule1.Verify(m => m.InitializeAsync(), Times.Once);
        mockModule2.Verify(m => m.InitializeAsync(), Times.Once);
    }

    [Fact]
    public void UnregisterModule_RemovesModuleFromRegistry()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();
        var mockModule = new Mock<IMicrofrontendModule>();
        mockModule.Setup(m => m.ModuleId).Returns("test-module");
        lifecycle.RegisterModule(mockModule.Object);

        // Act
        lifecycle.UnregisterModule("test-module");

        // Assert
        var module = lifecycle.GetModule("test-module");
        Assert.Null(module);
    }

    [Fact]
    public void UnregisterModule_DoesNotThrow_WhenModuleDoesNotExist()
    {
        // Arrange
        var lifecycle = new MicrofrontendLifecycle();

        // Act & Assert
        var exception = Record.Exception(() => lifecycle.UnregisterModule("non-existent-module"));
        Assert.Null(exception);
    }
} 