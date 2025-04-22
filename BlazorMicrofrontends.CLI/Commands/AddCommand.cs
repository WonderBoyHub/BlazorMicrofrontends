using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
using BlazorMicrofrontends.CLI.Services;

namespace BlazorMicrofrontends.CLI.Commands;

[Command(Name = "add", Description = "Adds a microfrontend to an app shell")]
public class AddCommand
{
    private readonly IMicrofrontendService _microfrontendService;
    
    [Option(Template = "-a|--app-shell", Description = "Path to the app shell directory")]
    public string AppShellDir { get; set; } = "";
    
    [Option(Template = "-m|--microfrontend", Description = "Path to the microfrontend directory or package ID")]
    public string MicrofrontendPath { get; set; } = "";
    
    [Option(Template = "--route", Description = "Route path to mount the microfrontend on")]
    public string Route { get; set; } = "";
    
    [Option(Template = "--id", Description = "ID to use for the microfrontend in the app shell")]
    public string Id { get; set; } = "";
    
    [Option(Template = "--from-repo", Description = "Download the microfrontend from a package repository")]
    public bool FromRepository { get; set; } = false;
    
    [Option(Template = "--repo", Description = "Repository URL to download the microfrontend from")]
    public string Repository { get; set; } = "";
    
    [Option(Template = "--version", Description = "Version of the microfrontend package to download")]
    public string Version { get; set; } = "";
    
    public AddCommand(IMicrofrontendService microfrontendService)
    {
        _microfrontendService = microfrontendService ?? throw new ArgumentNullException(nameof(microfrontendService));
    }
    
    public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
    {
        if (string.IsNullOrEmpty(AppShellDir))
        {
            AppShellDir = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the path to the app shell directory:")
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]Path cannot be empty[/]")
                    .Validate(path => !string.IsNullOrWhiteSpace(path) && Directory.Exists(path)));
        }
        
        if (string.IsNullOrEmpty(MicrofrontendPath))
        {
            MicrofrontendPath = AnsiConsole.Prompt(
                new TextPrompt<string>(FromRepository ? "Enter the microfrontend package ID:" : "Enter the path to the microfrontend directory:")
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]Path cannot be empty[/]")
                    .Validate(path => !string.IsNullOrWhiteSpace(path)));
        }
        
        var options = new Dictionary<string, string>();
        
        if (!string.IsNullOrEmpty(Route))
        {
            options["Route"] = Route;
        }
        else
        {
            Route = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the route path to mount the microfrontend on:")
                    .PromptStyle("green")
                    .DefaultValue($"/{Path.GetFileNameWithoutExtension(MicrofrontendPath)}")
                    .ValidationErrorMessage("[red]Route cannot be empty[/]")
                    .Validate(route => !string.IsNullOrWhiteSpace(route)));
            
            options["Route"] = Route;
        }
        
        if (!string.IsNullOrEmpty(Id))
        {
            options["Id"] = Id;
        }
        
        if (FromRepository)
        {
            if (string.IsNullOrEmpty(Repository))
            {
                Repository = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter the repository URL:")
                        .PromptStyle("green")
                        .DefaultValue("https://api.nuget.org/v3/index.json")
                        .ValidationErrorMessage("[red]Repository URL cannot be empty[/]")
                        .Validate(repo => !string.IsNullOrWhiteSpace(repo)));
            }
            
            options["FromRepository"] = "true";
            options["Repository"] = Repository;
            
            if (!string.IsNullOrEmpty(Version))
            {
                options["Version"] = Version;
            }
        }
        
        bool result = false;
        
        await AnsiConsole.Status()
            .StartAsync("Adding microfrontend to app shell...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.Status("Adding microfrontend...");
                
                result = await _microfrontendService.AddMicrofrontendToAppShellAsync(AppShellDir, MicrofrontendPath, options);
                
                if (result)
                {
                    ctx.Status("Microfrontend added successfully!");
                }
                else
                {
                    ctx.Status("Failed to add microfrontend");
                }
            });
        
        return result ? 0 : 1;
    }
} 