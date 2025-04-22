using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
using BlazorMicrofrontends.CLI.Services;

namespace BlazorMicrofrontends.CLI.Commands;

[Command(Name = "new", Description = "Creates a new app shell or microfrontend")]
[Subcommand(typeof(NewAppShellCommand))]
[Subcommand(typeof(NewMicrofrontendCommand))]
public class NewCommand
{
    public int OnExecute(CommandLineApplication app, IConsole console)
    {
        console.WriteLine("You must specify what to create. See --help for more information.");
        app.ShowHelp();
        return 1;
    }
    
    [Command(Name = "app-shell", Description = "Creates a new app shell")]
    public class NewAppShellCommand
    {
        private readonly IAppShellService _appShellService;
        private readonly ITemplateService _templateService;
        
        [Option(Template = "-t|--template", Description = "Template to use (EmptyServer, EmptyWebAssembly, EmptyMaui, SampleServer, SampleWebAssembly, SampleMaui)")]
        public string Template { get; set; } = "EmptyServer";
        
        [Option(Template = "-n|--name", Description = "Name of the app shell")]
        public string Name { get; set; } = "";
        
        [Option(Template = "-o|--output", Description = "Output directory")]
        public string OutputDir { get; set; } = "";
        
        [Option(Template = "--with-mf", Description = "Add sample microfrontends (blazor, blazormaui, react, python, all)")]
        public string WithMicrofrontends { get; set; } = "";
        
        public NewAppShellCommand(IAppShellService appShellService, ITemplateService templateService)
        {
            _appShellService = appShellService ?? throw new ArgumentNullException(nameof(appShellService));
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }
        
        public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter a name for your app shell:")
                        .PromptStyle("green")
                        .ValidationErrorMessage("[red]Name cannot be empty[/]")
                        .Validate(name => !string.IsNullOrWhiteSpace(name)));
            }
            
            if (string.IsNullOrEmpty(OutputDir))
            {
                OutputDir = Directory.GetCurrentDirectory();
            }
            
            if (!Directory.Exists(OutputDir))
            {
                Directory.CreateDirectory(OutputDir);
            }
            
            var availableTemplates = await _templateService.GetAppShellTemplatesAsync();
            if (!availableTemplates.Contains(Template))
            {
                Template = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an app shell template:")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more templates)[/]")
                        .AddChoices(availableTemplates));
            }
            
            var options = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(WithMicrofrontends))
            {
                options["WithMicrofrontends"] = WithMicrofrontends;
            }
            
            AnsiConsole.Status()
                .Start("Creating app shell...", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    ctx.Status("Creating app shell from template...");
                    
                    var result = _appShellService.CreateAppShellAsync(Template, Name, OutputDir, options).GetAwaiter().GetResult();
                    
                    if (result)
                    {
                        ctx.Status("App shell created successfully!");
                    }
                    else
                    {
                        ctx.Status("Failed to create app shell");
                    }
                });
            
            return 0;
        }
    }
    
    [Command(Name = "microfrontend", Description = "Creates a new microfrontend")]
    public class NewMicrofrontendCommand
    {
        private readonly IMicrofrontendService _microfrontendService;
        private readonly ITemplateService _templateService;
        
        [Option(Template = "-t|--template", Description = "Template to use (Blazor, BlazorMaui, React, Python)")]
        public string Template { get; set; } = "Blazor";
        
        [Option(Template = "-n|--name", Description = "Name of the microfrontend")]
        public string Name { get; set; } = "";
        
        [Option(Template = "-o|--output", Description = "Output directory")]
        public string OutputDir { get; set; } = "";
        
        // React options
        [Option(Template = "--language", Description = "For React: JavaScript or TypeScript")]
        public string Language { get; set; } = "JavaScript";
        
        [Option(Template = "--use-jsx", Description = "For React: Whether to use JSX/TSX syntax")]
        public bool UseJSX { get; set; } = true;
        
        [Option(Template = "--css-framework", Description = "For React: CSS framework (Bootstrap, TailwindCSS, None)")]
        public string CSSFramework { get; set; } = "Bootstrap";
        
        // Python options
        [Option(Template = "--framework", Description = "For Python: Web framework (Flask, Django)")]
        public string Framework { get; set; } = "Flask";
        
        [Option(Template = "--include-database", Description = "For Python: Whether to include database integration")]
        public bool IncludeDatabase { get; set; } = false;
        
        [Option(Template = "--database-type", Description = "For Python: Database type (SQLite, PostgreSQL)")]
        public string DatabaseType { get; set; } = "SQLite";
        
        public NewMicrofrontendCommand(IMicrofrontendService microfrontendService, ITemplateService templateService)
        {
            _microfrontendService = microfrontendService ?? throw new ArgumentNullException(nameof(microfrontendService));
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }
        
        public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter a name for your microfrontend:")
                        .PromptStyle("green")
                        .ValidationErrorMessage("[red]Name cannot be empty[/]")
                        .Validate(name => !string.IsNullOrWhiteSpace(name)));
            }
            
            if (string.IsNullOrEmpty(OutputDir))
            {
                OutputDir = Directory.GetCurrentDirectory();
            }
            
            if (!Directory.Exists(OutputDir))
            {
                Directory.CreateDirectory(OutputDir);
            }
            
            var availableTemplates = await _templateService.GetMicrofrontendTemplatesAsync();
            if (!availableTemplates.Contains(Template))
            {
                Template = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select a microfrontend template:")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more templates)[/]")
                        .AddChoices(availableTemplates));
            }
            
            var options = new Dictionary<string, string>();
            
            // Handle template-specific options
            if (Template == "React")
            {
                // For React template
                if (!new[] { "JavaScript", "TypeScript" }.Contains(Language))
                {
                    Language = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select a language for your React microfrontend:")
                            .AddChoices("JavaScript", "TypeScript"));
                }
                
                if (!new[] { "Bootstrap", "TailwindCSS", "None" }.Contains(CSSFramework))
                {
                    CSSFramework = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select a CSS framework:")
                            .AddChoices("Bootstrap", "TailwindCSS", "None"));
                }
                
                options["Language"] = Language;
                options["UseJSX"] = UseJSX.ToString();
                options["CSSFramework"] = CSSFramework;
            }
            else if (Template == "Python")
            {
                // For Python template
                if (!new[] { "Flask", "Django" }.Contains(Framework))
                {
                    Framework = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select a web framework for your Python microfrontend:")
                            .AddChoices("Flask", "Django"));
                }
                
                options["Framework"] = Framework;
                options["IncludeDatabase"] = IncludeDatabase.ToString();
                
                if (IncludeDatabase)
                {
                    if (!new[] { "SQLite", "PostgreSQL" }.Contains(DatabaseType))
                    {
                        DatabaseType = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Select a database type:")
                                .AddChoices("SQLite", "PostgreSQL"));
                    }
                    
                    options["DatabaseType"] = DatabaseType;
                }
            }
            
            AnsiConsole.WriteLine($"Creating a new {Template} microfrontend named {Name}...");
            
            bool success = await _microfrontendService.CreateMicrofrontendAsync(Template, Name, OutputDir, options);
            
            if (success)
            {
                AnsiConsole.WriteLine($"Successfully created {Template} microfrontend {Name} in {OutputDir}");
                return 0;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Failed to create microfrontend[/]");
                return 1;
            }
        }
    }
} 