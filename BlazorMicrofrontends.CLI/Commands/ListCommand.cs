using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
using BlazorMicrofrontends.CLI.Services;

namespace BlazorMicrofrontends.CLI.Commands;

[Command(Name = "list", Description = "Lists microfrontends in an app shell or available from a repository")]
[Subcommand(typeof(ListLocalCommand))]
[Subcommand(typeof(ListRepositoryCommand))]
public class ListCommand
{
    public int OnExecute(CommandLineApplication app, IConsole console)
    {
        console.WriteLine("You must specify what to list. See --help for more information.");
        app.ShowHelp();
        return 1;
    }
    
    [Command(Name = "local", Description = "Lists microfrontends in an app shell")]
    public class ListLocalCommand
    {
        private readonly IAppShellService _appShellService;
        private readonly IMicrofrontendService _microfrontendService;
        
        [Option(Template = "-a|--app-shell", Description = "Path to the app shell directory")]
        public string AppShellDir { get; set; } = "";
        
        [Option(Template = "--json", Description = "Output as JSON")]
        public bool JsonOutput { get; set; } = false;
        
        [Option(Template = "--detailed", Description = "Show detailed information for each microfrontend")]
        public bool Detailed { get; set; } = false;
        
        public ListLocalCommand(IAppShellService appShellService, IMicrofrontendService microfrontendService)
        {
            _appShellService = appShellService ?? throw new ArgumentNullException(nameof(appShellService));
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
            
            var microfrontends = await _appShellService.ListMicrofrontendsAsync(AppShellDir);
            
            if (!microfrontends.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No microfrontends found in the app shell[/]");
                return 0;
            }
            
            if (JsonOutput)
            {
                // Output as JSON if requested
                console.WriteLine("{");
                console.WriteLine("  \"microfrontends\": [");
                
                int count = 0;
                foreach (var mf in microfrontends)
                {
                    count++;
                    if (Detailed)
                    {
                        var details = await _microfrontendService.GetMicrofrontendInfoAsync(Path.Combine(AppShellDir, mf));
                        console.WriteLine($"    {{");
                        console.WriteLine($"      \"id\": \"{mf}\",");
                        
                        int detailCount = 0;
                        foreach (var detail in details)
                        {
                            detailCount++;
                            console.WriteLine($"      \"{detail.Key}\": \"{detail.Value}\"{(detailCount < details.Count ? "," : "")}");
                        }
                        
                        console.WriteLine($"    }}{(count < microfrontends.Count() ? "," : "")}");
                    }
                    else
                    {
                        console.WriteLine($"    {{ \"id\": \"{mf}\" }}{(count < microfrontends.Count() ? "," : "")}");
                    }
                }
                
                console.WriteLine("  ]");
                console.WriteLine("}");
            }
            else
            {
                // Output as a table
                var table = new Table();
                table.AddColumn("ID");
                
                if (Detailed)
                {
                    table.AddColumn("Name");
                    table.AddColumn("Version");
                    table.AddColumn("Technology");
                    table.AddColumn("Route");
                    table.AddColumn("Enabled");
                }
                
                foreach (var mf in microfrontends)
                {
                    if (Detailed)
                    {
                        var details = await _microfrontendService.GetMicrofrontendInfoAsync(Path.Combine(AppShellDir, mf));
                        
                        table.AddRow(
                            mf,
                            details.TryGetValue("Name", out var name) ? name : "",
                            details.TryGetValue("Version", out var version) ? version : "",
                            details.TryGetValue("Technology", out var tech) ? tech : "",
                            details.TryGetValue("Route", out var route) ? route : "",
                            details.TryGetValue("Enabled", out var enabled) ? enabled : "true"
                        );
                    }
                    else
                    {
                        table.AddRow(mf);
                    }
                }
                
                AnsiConsole.Write(table);
            }
            
            return 0;
        }
    }
    
    [Command(Name = "repo", Description = "Lists available microfrontends from a repository")]
    public class ListRepositoryCommand
    {
        private readonly IPackageService _packageService;
        
        [Option(Template = "-r|--repo", Description = "Repository URL to list microfrontends from")]
        public string Repository { get; set; } = "https://api.nuget.org/v3/index.json";
        
        [Option(Template = "-s|--search", Description = "Search term to filter microfrontends")]
        public string SearchTerm { get; set; } = "";
        
        [Option(Template = "--json", Description = "Output as JSON")]
        public bool JsonOutput { get; set; } = false;
        
        public ListRepositoryCommand(IPackageService packageService)
        {
            _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
        }
        
        public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
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
            
            var packages = await _packageService.ListAvailablePackagesAsync(Repository, SearchTerm);
            
            if (!packages.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No microfrontends found in the repository[/]");
                return 0;
            }
            
            if (JsonOutput)
            {
                // Output as JSON if requested
                console.WriteLine("{");
                console.WriteLine("  \"packages\": [");
                
                int count = 0;
                foreach (var pkg in packages)
                {
                    count++;
                    console.WriteLine($"    {{ \"id\": \"{pkg}\" }}{(count < packages.Count() ? "," : "")}");
                }
                
                console.WriteLine("  ]");
                console.WriteLine("}");
            }
            else
            {
                // Output as a table
                var table = new Table();
                table.AddColumn("Package ID");
                
                foreach (var pkg in packages)
                {
                    table.AddRow(pkg);
                }
                
                AnsiConsole.Write(table);
            }
            
            return 0;
        }
    }
} 