using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using System.Reflection;
using BlazorMicrofrontends.CLI.Commands;
using BlazorMicrofrontends.CLI.Services;
using BlazorMicrofrontends.CLI.Services.Implementations;

namespace BlazorMicrofrontends.CLI;

[Command(Name = "bmf", Description = "Blazor Microfrontends CLI tool for managing microfrontend applications")]
[Subcommand(typeof(NewCommand))]
[Subcommand(typeof(ListCommand))]
[Subcommand(typeof(AddCommand))]
[Subcommand(typeof(RemoveCommand))]
[Subcommand(typeof(PublishCommand))]
[Subcommand(typeof(PackageCommand))]
[Subcommand(typeof(EnableCommand))]
[Subcommand(typeof(DisableCommand))]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
class Program
{
    [Option(Template = "-v|--verbose", Description = "Enable verbose output")]
    public bool Verbose { get; set; }

    static async Task<int> Main(string[] args)
    {
        AnsiConsole.Write(
            new FigletText("BMF CLI")
                .Color(Color.Blue));

        try
        {
            var host = CreateHostBuilder(args).Build();
            return await host.Services.GetRequiredService<CommandLineApplication<Program>>().ExecuteAsync(args);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ITemplateService, TemplateService>();
                services.AddSingleton<IAppShellService, AppShellService>();
                services.AddSingleton<IMicrofrontendService, MicrofrontendService>();
                services.AddSingleton<IPackageService, PackageService>();
                
                services.AddSingleton<CommandLineApplication<Program>>(provider =>
                {
                    var app = new CommandLineApplication<Program>();
                    app.Conventions
                        .UseDefaultConventions()
                        .UseConstructorInjection(provider);
                    return app;
                });
            });

    private int OnExecute(CommandLineApplication app, IConsole console)
    {
        console.WriteLine("You must specify a command. See --help for more information.");
        app.ShowHelp();
        return 1;
    }

    private static string GetVersion()
        => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? typeof(Program).Assembly.GetName().Version?.ToString()
        ?? "0.0.1";
}
