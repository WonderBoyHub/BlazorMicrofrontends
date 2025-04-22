using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
using BlazorMicrofrontends.CLI.Services;

namespace BlazorMicrofrontends.CLI.Commands;

[Command(Name = "enable", Description = "Enables a microfrontend in an app shell")]
public class EnableCommand
{
    private readonly IMicrofrontendService _microfrontendService;
    private readonly IAppShellService _appShellService;
    
    [Option(Template = "-a|--app-shell", Description = "Path to the app shell directory")]
    public string AppShellDir { get; set; } = "";
    
    [Option(Template = "-m|--microfrontend", Description = "ID of the microfrontend to enable")]
    public string MicrofrontendId { get; set; } = "";
    
    public EnableCommand(IMicrofrontendService microfrontendService, IAppShellService appShellService)
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
            AnsiConsole.MarkupLine("[red]No microfrontends found in the app shell[/]");
            return 1;
        }
        
        if (string.IsNullOrEmpty(MicrofrontendId))
        {
            var disabledMicrofrontends = new List<string>();
            
            foreach (var mf in microfrontends)
            {
                var info = await _microfrontendService.GetMicrofrontendInfoAsync(Path.Combine(AppShellDir, mf));
                if (info.TryGetValue("Enabled", out var enabled) && enabled.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    disabledMicrofrontends.Add(mf);
                }
            }
            
            if (!disabledMicrofrontends.Any())
            {
                AnsiConsole.MarkupLine("[yellow]All microfrontends are already enabled in the app shell[/]");
                return 0;
            }
            
            MicrofrontendId = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the microfrontend to enable:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more microfrontends)[/]")
                    .AddChoices(disabledMicrofrontends));
        }
        
        if (!microfrontends.Contains(MicrofrontendId))
        {
            AnsiConsole.MarkupLine($"[red]Microfrontend with ID '{MicrofrontendId}' not found in the app shell[/]");
            return 1;
        }
        
        var microfrontendInfo = await _microfrontendService.GetMicrofrontendInfoAsync(Path.Combine(AppShellDir, MicrofrontendId));
        if (!microfrontendInfo.TryGetValue("Enabled", out var isEnabled) || !isEnabled.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine($"[yellow]Microfrontend '{MicrofrontendId}' is already enabled[/]");
            return 0;
        }
        
        await _microfrontendService.SetMicrofrontendEnabledAsync(AppShellDir, MicrofrontendId, true);
        
        AnsiConsole.MarkupLine($"[green]Successfully enabled microfrontend '{MicrofrontendId}' in the app shell[/]");
        
        return 0;
    }
} 