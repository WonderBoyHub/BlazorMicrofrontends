using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
using BlazorMicrofrontends.CLI.Services;

namespace BlazorMicrofrontends.CLI.Commands;

[Command(Name = "remove", Description = "Removes a microfrontend from an app shell")]
public class RemoveCommand
{
    private readonly IMicrofrontendService _microfrontendService;
    private readonly IAppShellService _appShellService;
    
    [Option(Template = "-a|--app-shell", Description = "Path to the app shell directory")]
    public string AppShellDir { get; set; } = "";
    
    [Option(Template = "-m|--microfrontend", Description = "ID of the microfrontend to remove")]
    public string MicrofrontendId { get; set; } = "";
    
    [Option(Template = "--disable-only", Description = "Only disable the microfrontend instead of removing it")]
    public bool DisableOnly { get; set; } = false;
    
    public RemoveCommand(IMicrofrontendService microfrontendService, IAppShellService appShellService)
    {
        _microfrontendService = microfrontendService ?? throw new ArgumentNullException(nameof(microfrontendService));
        _appShellService = appShellService ?? throw new ArgumentNullException(nameof(appShellService));
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
        
        var microfrontends = await _appShellService.ListMicrofrontendsAsync(AppShellDir);
        
        if (!microfrontends.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No microfrontends found in the app shell[/]");
            return 0;
        }
        
        if (string.IsNullOrEmpty(MicrofrontendId))
        {
            MicrofrontendId = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a microfrontend to remove:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more microfrontends)[/]")
                    .AddChoices(microfrontends));
        }
        else if (!microfrontends.Contains(MicrofrontendId))
        {
            AnsiConsole.MarkupLine($"[red]Microfrontend with ID '{MicrofrontendId}' not found in the app shell[/]");
            return 1;
        }
        
        bool result = false;
        
        if (DisableOnly)
        {
            await AnsiConsole.Status()
                .StartAsync("Disabling microfrontend...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    
                    result = await _microfrontendService.SetMicrofrontendEnabledAsync(AppShellDir, MicrofrontendId, false);
                    
                    if (result)
                    {
                        ctx.Status("Microfrontend disabled successfully!");
                    }
                    else
                    {
                        ctx.Status("Failed to disable microfrontend");
                    }
                });
        }
        else
        {
            if (!AnsiConsole.Confirm($"Are you sure you want to remove the microfrontend with ID '{MicrofrontendId}'?"))
            {
                return 0;
            }
            
            await AnsiConsole.Status()
                .StartAsync("Removing microfrontend...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    
                    result = await _microfrontendService.RemoveMicrofrontendFromAppShellAsync(AppShellDir, MicrofrontendId);
                    
                    if (result)
                    {
                        ctx.Status("Microfrontend removed successfully!");
                    }
                    else
                    {
                        ctx.Status("Failed to remove microfrontend");
                    }
                });
        }
        
        return result ? 0 : 1;
    }
} 