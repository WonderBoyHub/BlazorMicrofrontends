using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
using BlazorMicrofrontends.CLI.Services;

namespace BlazorMicrofrontends.CLI.Commands;

[Command(Name = "publish", Description = "Publishes an app shell or microfrontend as a NuGet package")]
[Subcommand(typeof(PublishAppShellCommand))]
[Subcommand(typeof(PublishMicrofrontendCommand))]
public class PublishCommand
{
    public int OnExecute(CommandLineApplication app, IConsole console)
    {
        console.WriteLine("You must specify what to publish. See --help for more information.");
        app.ShowHelp();
        return 1;
    }
    
    [Command(Name = "app-shell", Description = "Publishes an app shell as a NuGet package")]
    public class PublishAppShellCommand
    {
        private readonly IAppShellService _appShellService;
        private readonly IPackageService _packageService;
        
        [Option(Template = "-p|--path", Description = "Path to the app shell directory")]
        public string AppShellDir { get; set; } = "";
        
        [Option(Template = "-o|--output", Description = "Output directory for the package")]
        public string OutputDir { get; set; } = "";
        
        [Option(Template = "-r|--repo", Description = "Repository URL to publish to")]
        public string Repository { get; set; } = "";
        
        [Option(Template = "-k|--api-key", Description = "API key for the repository")]
        public string ApiKey { get; set; } = "";
        
        [Option(Template = "--version", Description = "Version to publish")]
        public string Version { get; set; } = "";
        
        [Option(Template = "--description", Description = "Description of the package")]
        public string Description { get; set; } = "";
        
        [Option(Template = "--authors", Description = "Authors of the package")]
        public string Authors { get; set; } = "";
        
        [Option(Template = "--tags", Description = "Tags for the package")]
        public string Tags { get; set; } = "";
        
        public PublishAppShellCommand(IAppShellService appShellService, IPackageService packageService)
        {
            _appShellService = appShellService ?? throw new ArgumentNullException(nameof(appShellService));
            _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
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
            
            if (string.IsNullOrEmpty(OutputDir))
            {
                OutputDir = Path.Combine(Directory.GetCurrentDirectory(), "packages");
            }
            
            if (!Directory.Exists(OutputDir))
            {
                Directory.CreateDirectory(OutputDir);
            }
            
            if (!string.IsNullOrEmpty(Repository) && string.IsNullOrEmpty(ApiKey))
            {
                ApiKey = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter the API key for the repository:")
                        .PromptStyle("green")
                        .Secret()
                        .ValidationErrorMessage("[red]API key cannot be empty[/]")
                        .Validate(key => !string.IsNullOrWhiteSpace(key)));
            }
            
            var options = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(Version))
            {
                options["Version"] = Version;
            }
            
            if (!string.IsNullOrEmpty(Description))
            {
                options["Description"] = Description;
            }
            
            if (!string.IsNullOrEmpty(Authors))
            {
                options["Authors"] = Authors;
            }
            
            if (!string.IsNullOrEmpty(Tags))
            {
                options["Tags"] = Tags;
            }
            
            if (!string.IsNullOrEmpty(Repository))
            {
                options["Repository"] = Repository;
                options["ApiKey"] = ApiKey;
            }
            
            options["OutputDir"] = OutputDir;
            
            bool result = false;
            
            await AnsiConsole.Status()
                .StartAsync("Publishing app shell...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    ctx.Status("Creating package...");
                    
                    result = await _appShellService.PublishAppShellAsync(AppShellDir, options);
                    
                    if (result)
                    {
                        if (!string.IsNullOrEmpty(Repository))
                        {
                            ctx.Status("Package created and published successfully!");
                        }
                        else
                        {
                            ctx.Status($"Package created successfully at {OutputDir}");
                        }
                    }
                    else
                    {
                        ctx.Status("Failed to publish app shell");
                    }
                });
            
            return result ? 0 : 1;
        }
    }
    
    [Command(Name = "microfrontend", Description = "Publishes a microfrontend as a NuGet package")]
    public class PublishMicrofrontendCommand
    {
        private readonly IMicrofrontendService _microfrontendService;
        private readonly IPackageService _packageService;
        
        [Option(Template = "-p|--path", Description = "Path to the microfrontend directory")]
        public string MicrofrontendDir { get; set; } = "";
        
        [Option(Template = "-o|--output", Description = "Output directory for the package")]
        public string OutputDir { get; set; } = "";
        
        [Option(Template = "-r|--repo", Description = "Repository URL to publish to")]
        public string Repository { get; set; } = "";
        
        [Option(Template = "-k|--api-key", Description = "API key for the repository")]
        public string ApiKey { get; set; } = "";
        
        [Option(Template = "--version", Description = "Version to publish")]
        public string Version { get; set; } = "";
        
        [Option(Template = "--description", Description = "Description of the package")]
        public string Description { get; set; } = "";
        
        [Option(Template = "--authors", Description = "Authors of the package")]
        public string Authors { get; set; } = "";
        
        [Option(Template = "--tags", Description = "Tags for the package")]
        public string Tags { get; set; } = "";
        
        public PublishMicrofrontendCommand(IMicrofrontendService microfrontendService, IPackageService packageService)
        {
            _microfrontendService = microfrontendService ?? throw new ArgumentNullException(nameof(microfrontendService));
            _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
        }
        
        public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            if (string.IsNullOrEmpty(MicrofrontendDir))
            {
                MicrofrontendDir = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter the path to the microfrontend directory:")
                        .PromptStyle("green")
                        .ValidationErrorMessage("[red]Path cannot be empty[/]")
                        .Validate(path => !string.IsNullOrWhiteSpace(path) && Directory.Exists(path)));
            }
            
            if (string.IsNullOrEmpty(OutputDir))
            {
                OutputDir = Path.Combine(Directory.GetCurrentDirectory(), "packages");
            }
            
            if (!Directory.Exists(OutputDir))
            {
                Directory.CreateDirectory(OutputDir);
            }
            
            if (!string.IsNullOrEmpty(Repository) && string.IsNullOrEmpty(ApiKey))
            {
                ApiKey = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter the API key for the repository:")
                        .PromptStyle("green")
                        .Secret()
                        .ValidationErrorMessage("[red]API key cannot be empty[/]")
                        .Validate(key => !string.IsNullOrWhiteSpace(key)));
            }
            
            var options = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(Version))
            {
                options["Version"] = Version;
            }
            
            if (!string.IsNullOrEmpty(Description))
            {
                options["Description"] = Description;
            }
            
            if (!string.IsNullOrEmpty(Authors))
            {
                options["Authors"] = Authors;
            }
            
            if (!string.IsNullOrEmpty(Tags))
            {
                options["Tags"] = Tags;
            }
            
            if (!string.IsNullOrEmpty(Repository))
            {
                options["Repository"] = Repository;
                options["ApiKey"] = ApiKey;
            }
            
            options["OutputDir"] = OutputDir;
            
            bool result = false;
            
            await AnsiConsole.Status()
                .StartAsync("Publishing microfrontend...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    ctx.Status("Creating package...");
                    
                    result = await _microfrontendService.PublishMicrofrontendAsync(MicrofrontendDir, options);
                    
                    if (result)
                    {
                        if (!string.IsNullOrEmpty(Repository))
                        {
                            ctx.Status("Package created and published successfully!");
                        }
                        else
                        {
                            ctx.Status($"Package created successfully at {OutputDir}");
                        }
                    }
                    else
                    {
                        ctx.Status("Failed to publish microfrontend");
                    }
                });
            
            return result ? 0 : 1;
        }
    }
} 