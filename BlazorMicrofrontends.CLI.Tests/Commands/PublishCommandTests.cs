using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorMicrofrontends.CLI.Commands;
using BlazorMicrofrontends.CLI.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.CLI.Tests.Commands;

public class PublishCommandTests
{
    private readonly Mock<IAppShellService> _mockAppShellService;
    private readonly Mock<IMicrofrontendService> _mockMicrofrontendService;
    private readonly Mock<ILogger<PublishCommand>> _mockLogger;
    private readonly Mock<IConsole> _mockConsole;
    
    public PublishCommandTests()
    {
        _mockAppShellService = new Mock<IAppShellService>();
        _mockMicrofrontendService = new Mock<IMicrofrontendService>();
        _mockLogger = new Mock<ILogger<PublishCommand>>();
        _mockConsole = new Mock<IConsole>();
    }
    
    [Fact]
    public void OnExecute_ReturnsOne_WhenNoSubcommandSpecified()
    {
        // Arrange
        var command = new PublishCommand(
            _mockAppShellService.Object,
            _mockMicrofrontendService.Object,
            _mockLogger.Object);
        
        // Act
        var result = command.OnExecute();
        
        // Assert
        Assert.Equal(1, result);
    }
    
    [Fact]
    public async Task PublishAppShellCommand_OnExecuteAsync_PublishesAppShell()
    {
        // Arrange
        var command = new PublishAppShellCommand(
            _mockAppShellService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.AppShellDir = "./appshell";
        command.OutputDir = "./output";
        command.Repository = "https://nuget.org/api/v2";
        command.ApiKey = "api-key";
        command.Version = "1.0.0";
        command.Authors = "Test Author";
        command.Description = "Test Description";
        command.Tags = "tag1 tag2";
        
        _mockAppShellService.Setup(s => s.PublishAppShellAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("./output/appshell.1.0.0.nupkg");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        _mockAppShellService.Verify(s => s.PublishAppShellAsync(
            "./appshell", 
            "./output", 
            It.Is<Dictionary<string, string>>(d => 
                d.ContainsKey("repository") && 
                d.ContainsKey("apiKey") && 
                d.ContainsKey("version") &&
                d.ContainsKey("authors") &&
                d.ContainsKey("description") &&
                d.ContainsKey("tags"))), 
            Times.Once);
    }
    
    [Fact]
    public async Task PublishAppShellCommand_OnExecuteAsync_PromptsForMissingAppShellDir()
    {
        // Arrange
        var prompter = new Mock<IPrompt>();
        prompter.Setup(p => p.GetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns("./prompted-appshell");
        
        _mockConsole.Setup(c => c.GetPrompt())
            .Returns(prompter.Object);
        
        var command = new PublishAppShellCommand(
            _mockAppShellService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.AppShellDir = null; // Missing app shell dir
        command.OutputDir = "./output";
        
        _mockAppShellService.Setup(s => s.PublishAppShellAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("./output/appshell.1.0.0.nupkg");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        prompter.Verify(p => p.GetString("Enter app shell directory:", It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _mockAppShellService.Verify(s => s.PublishAppShellAsync(
            "./prompted-appshell", 
            "./output", 
            It.IsAny<Dictionary<string, string>>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task PublishAppShellCommand_OnExecuteAsync_PromptsForApiKey_WhenRepositoryProvided()
    {
        // Arrange
        var prompter = new Mock<IPrompt>();
        prompter.Setup(p => p.GetPassword(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns("prompted-api-key");
        
        _mockConsole.Setup(c => c.GetPrompt())
            .Returns(prompter.Object);
        
        var command = new PublishAppShellCommand(
            _mockAppShellService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.AppShellDir = "./appshell";
        command.OutputDir = "./output";
        command.Repository = "https://nuget.org/api/v2";
        command.ApiKey = null; // Missing API key
        
        _mockAppShellService.Setup(s => s.PublishAppShellAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("./output/appshell.1.0.0.nupkg");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        prompter.Verify(p => p.GetPassword("Enter API key:", It.IsAny<bool>()), Times.Once);
        _mockAppShellService.Verify(s => s.PublishAppShellAsync(
            "./appshell", 
            "./output", 
            It.Is<Dictionary<string, string>>(d => 
                d.ContainsKey("repository") && 
                d["repository"] == "https://nuget.org/api/v2" &&
                d.ContainsKey("apiKey") && 
                d["apiKey"] == "prompted-api-key")), 
            Times.Once);
    }
    
    [Fact]
    public async Task PublishMicrofrontendCommand_OnExecuteAsync_PublishesMicrofrontend()
    {
        // Arrange
        var command = new PublishMicrofrontendCommand(
            _mockMicrofrontendService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.MicrofrontendDir = "./microfrontend";
        command.OutputDir = "./output";
        command.Repository = "https://nuget.org/api/v2";
        command.ApiKey = "api-key";
        command.Version = "1.0.0";
        command.Authors = "Test Author";
        command.Description = "Test Description";
        command.Tags = "tag1 tag2";
        
        _mockMicrofrontendService.Setup(s => s.PublishMicrofrontendAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("./output/microfrontend.1.0.0.nupkg");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        _mockMicrofrontendService.Verify(s => s.PublishMicrofrontendAsync(
            "./microfrontend", 
            "./output", 
            It.Is<Dictionary<string, string>>(d => 
                d.ContainsKey("repository") && 
                d.ContainsKey("apiKey") && 
                d.ContainsKey("version") &&
                d.ContainsKey("authors") &&
                d.ContainsKey("description") &&
                d.ContainsKey("tags"))), 
            Times.Once);
    }
    
    [Fact]
    public async Task PublishMicrofrontendCommand_OnExecuteAsync_PromptsForMissingMicrofrontendDir()
    {
        // Arrange
        var prompter = new Mock<IPrompt>();
        prompter.Setup(p => p.GetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns("./prompted-microfrontend");
        
        _mockConsole.Setup(c => c.GetPrompt())
            .Returns(prompter.Object);
        
        var command = new PublishMicrofrontendCommand(
            _mockMicrofrontendService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.MicrofrontendDir = null; // Missing microfrontend dir
        command.OutputDir = "./output";
        
        _mockMicrofrontendService.Setup(s => s.PublishMicrofrontendAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("./output/microfrontend.1.0.0.nupkg");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        prompter.Verify(p => p.GetString("Enter microfrontend directory:", It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _mockMicrofrontendService.Verify(s => s.PublishMicrofrontendAsync(
            "./prompted-microfrontend", 
            "./output", 
            It.IsAny<Dictionary<string, string>>()), 
            Times.Once);
    }
} 