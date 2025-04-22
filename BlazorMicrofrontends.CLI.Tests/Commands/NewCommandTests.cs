using System.Threading.Tasks;
using BlazorMicrofrontends.CLI.Commands;
using BlazorMicrofrontends.CLI.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BlazorMicrofrontends.CLI.Tests.Commands;

public class NewCommandTests
{
    private readonly Mock<IAppShellService> _mockAppShellService;
    private readonly Mock<IMicrofrontendService> _mockMicrofrontendService;
    private readonly Mock<ITemplateService> _mockTemplateService;
    private readonly Mock<ILogger<NewCommand>> _mockLogger;
    private readonly Mock<IConsole> _mockConsole;
    
    public NewCommandTests()
    {
        _mockAppShellService = new Mock<IAppShellService>();
        _mockMicrofrontendService = new Mock<IMicrofrontendService>();
        _mockTemplateService = new Mock<ITemplateService>();
        _mockLogger = new Mock<ILogger<NewCommand>>();
        _mockConsole = new Mock<IConsole>();
    }
    
    [Fact]
    public void OnExecute_ReturnsOne_WhenNoSubcommandSpecified()
    {
        // Arrange
        var command = new NewCommand(
            _mockAppShellService.Object,
            _mockMicrofrontendService.Object,
            _mockTemplateService.Object,
            _mockLogger.Object);
        
        // Act
        var result = command.OnExecute();
        
        // Assert
        Assert.Equal(1, result);
    }
    
    [Fact]
    public async Task NewAppShellCommand_OnExecuteAsync_CreatesAppShell()
    {
        // Arrange
        var command = new NewAppShellCommand(
            _mockAppShellService.Object,
            _mockTemplateService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.Template = "EmptyServer";
        command.Name = "MyTestAppShell";
        command.OutputDir = "./output";
        command.IncludeSamples = true;
        
        _mockTemplateService.Setup(t => t.GetAppShellTemplatesAsync())
            .ReturnsAsync(new[] { "EmptyServer", "EmptyWebAssembly" });
        
        _mockAppShellService.Setup(s => s.CreateAppShellAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync("./output/MyTestAppShell");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        _mockAppShellService.Verify(s => s.CreateAppShellAsync(
            "EmptyServer", "MyTestAppShell", "./output", true), Times.Once);
    }
    
    [Fact]
    public async Task NewAppShellCommand_OnExecuteAsync_ValidatesTemplateName()
    {
        // Arrange
        var command = new NewAppShellCommand(
            _mockAppShellService.Object,
            _mockTemplateService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.Template = "InvalidTemplate";
        command.Name = "MyTestAppShell";
        command.OutputDir = "./output";
        
        _mockTemplateService.Setup(t => t.GetAppShellTemplatesAsync())
            .ReturnsAsync(new[] { "EmptyServer", "EmptyWebAssembly" });
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(1, result);
        _mockAppShellService.Verify(s => s.CreateAppShellAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }
    
    [Fact]
    public async Task NewMicrofrontendCommand_OnExecuteAsync_CreatesMicrofrontend()
    {
        // Arrange
        var command = new NewMicrofrontendCommand(
            _mockMicrofrontendService.Object,
            _mockTemplateService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.Template = "Blazor";
        command.Name = "MyTestMicrofrontend";
        command.OutputDir = "./output";
        
        _mockTemplateService.Setup(t => t.GetMicrofrontendTemplatesAsync())
            .ReturnsAsync(new[] { "Blazor", "React" });
        
        _mockMicrofrontendService.Setup(s => s.CreateMicrofrontendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("./output/MyTestMicrofrontend");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        _mockMicrofrontendService.Verify(s => s.CreateMicrofrontendAsync(
            "Blazor", "MyTestMicrofrontend", "./output"), Times.Once);
    }
    
    [Fact]
    public async Task NewMicrofrontendCommand_OnExecuteAsync_ValidatesTemplateName()
    {
        // Arrange
        var command = new NewMicrofrontendCommand(
            _mockMicrofrontendService.Object,
            _mockTemplateService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.Template = "InvalidTemplate";
        command.Name = "MyTestMicrofrontend";
        command.OutputDir = "./output";
        
        _mockTemplateService.Setup(t => t.GetMicrofrontendTemplatesAsync())
            .ReturnsAsync(new[] { "Blazor", "React" });
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(1, result);
        _mockMicrofrontendService.Verify(s => s.CreateMicrofrontendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task NewAppShellCommand_OnExecuteAsync_PromptsForMissingName()
    {
        // Arrange
        var prompter = new Mock<IPrompt>();
        prompter.Setup(p => p.GetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns("PromptedName");
        
        _mockConsole.Setup(c => c.GetPrompt())
            .Returns(prompter.Object);
        
        var command = new NewAppShellCommand(
            _mockAppShellService.Object,
            _mockTemplateService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.Template = "EmptyServer";
        command.Name = null; // Missing name
        command.OutputDir = "./output";
        
        _mockTemplateService.Setup(t => t.GetAppShellTemplatesAsync())
            .ReturnsAsync(new[] { "EmptyServer", "EmptyWebAssembly" });
        
        _mockAppShellService.Setup(s => s.CreateAppShellAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync("./output/PromptedName");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        prompter.Verify(p => p.GetString("Enter app shell name:", It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _mockAppShellService.Verify(s => s.CreateAppShellAsync(
            "EmptyServer", "PromptedName", "./output", false), Times.Once);
    }
    
    [Fact]
    public async Task NewMicrofrontendCommand_OnExecuteAsync_PromptsForMissingName()
    {
        // Arrange
        var prompter = new Mock<IPrompt>();
        prompter.Setup(p => p.GetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns("PromptedName");
        
        _mockConsole.Setup(c => c.GetPrompt())
            .Returns(prompter.Object);
        
        var command = new NewMicrofrontendCommand(
            _mockMicrofrontendService.Object,
            _mockTemplateService.Object,
            _mockLogger.Object,
            _mockConsole.Object);
        
        command.Template = "Blazor";
        command.Name = null; // Missing name
        command.OutputDir = "./output";
        
        _mockTemplateService.Setup(t => t.GetMicrofrontendTemplatesAsync())
            .ReturnsAsync(new[] { "Blazor", "React" });
        
        _mockMicrofrontendService.Setup(s => s.CreateMicrofrontendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("./output/PromptedName");
        
        // Act
        var result = await command.OnExecuteAsync();
        
        // Assert
        Assert.Equal(0, result);
        prompter.Verify(p => p.GetString("Enter microfrontend name:", It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _mockMicrofrontendService.Verify(s => s.CreateMicrofrontendAsync(
            "Blazor", "PromptedName", "./output"), Times.Once);
    }
} 