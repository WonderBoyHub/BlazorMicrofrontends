using System.Threading.Tasks;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.Core.Tests;

public class MicrofrontendModuleBaseTests
{
    private class TestMicrofrontendModule : MicrofrontendModuleBase
    {
        public override string ModuleId { get; }
        public override string Name { get; }
        public override string Version { get; }
        public override string Technology { get; }

        public TestMicrofrontendModule(string moduleId, string name, string version, string technology)
        {
            ModuleId = moduleId;
            Name = name;
            Version = version;
            Technology = technology;
        }

        public bool InitializeCalled { get; private set; }
        public bool CleanupCalled { get; private set; }

        protected override Task OnInitializeAsync()
        {
            InitializeCalled = true;
            return Task.CompletedTask;
        }

        protected override Task OnCleanupAsync()
        {
            CleanupCalled = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InitializeAsync_ShouldCallOnInitializeAsync()
    {
        // Arrange
        var module = new TestMicrofrontendModule("test-module", "Test Module", "1.0.0", "test");

        // Act
        await module.InitializeAsync();

        // Assert
        Assert.True(module.InitializeCalled);
        Assert.True(module.IsInitialized);
    }

    [Fact]
    public async Task InitializeAsync_ShouldOnlyCallOnInitializeAsyncOnce()
    {
        // Arrange
        var module = new TestMicrofrontendModule("test-module", "Test Module", "1.0.0", "test");

        // Act
        await module.InitializeAsync();
        module.InitializeCalled = false; // Reset flag
        await module.InitializeAsync(); // Call again

        // Assert
        Assert.False(module.InitializeCalled); // Should not be called second time
        Assert.True(module.IsInitialized);
    }

    [Fact]
    public async Task CleanupAsync_ShouldCallOnCleanupAsync()
    {
        // Arrange
        var module = new TestMicrofrontendModule("test-module", "Test Module", "1.0.0", "test");
        await module.InitializeAsync(); // Initialize first

        // Act
        await module.CleanupAsync();

        // Assert
        Assert.True(module.CleanupCalled);
        Assert.False(module.IsInitialized);
    }

    [Fact]
    public async Task CleanupAsync_ShouldNotCallOnCleanupAsync_WhenNotInitialized()
    {
        // Arrange
        var module = new TestMicrofrontendModule("test-module", "Test Module", "1.0.0", "test");

        // Act
        await module.CleanupAsync();

        // Assert
        Assert.False(module.CleanupCalled);
        Assert.False(module.IsInitialized);
    }

    [Fact]
    public void Routes_ShouldBeEmpty_ByDefault()
    {
        // Arrange & Act
        var module = new TestMicrofrontendModule("test-module", "Test Module", "1.0.0", "test");

        // Assert
        Assert.Empty(module.Routes);
    }

    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange & Act
        var module = new TestMicrofrontendModule("test-module", "Test Module", "1.0.0", "test");

        // Assert
        Assert.Equal("test-module", module.ModuleId);
        Assert.Equal("Test Module", module.Name);
        Assert.Equal("1.0.0", module.Version);
        Assert.Equal("test", module.Technology);
        Assert.False(module.IsInitialized);
    }
} 