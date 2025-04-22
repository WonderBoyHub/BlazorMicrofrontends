using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using BlazorMicrofrontends.CLI.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace BlazorMicrofrontends.CLI.Commands;

/// <summary>
/// Command to package microfrontends and app shells.
/// </summary>
public class PackageCommand : Command
{
    private readonly Option<string> _dirOption;
    private readonly Option<string> _outputOption;
    private readonly Option<string> _versionOption;
    private readonly Option<string> _configOption;
    private readonly Option<string> _optimizationOption;

    public PackageCommand() : base("package", "Package microfrontends and app shells for distribution")
    {
        var microfrontendCommand = new Command("microfrontend", "Package a microfrontend");
        var appshellCommand = new Command("appshell", "Package an app shell");

        // Shared options
        _dirOption = new Option<string>(
            aliases: new[] { "--dir", "-d" },
            description: "The directory containing the microfrontend or app shell")
        {
            IsRequired = false
        };

        _outputOption = new Option<string>(
            aliases: new[] { "--output", "-o" },
            description: "Output directory for the package")
        {
            IsRequired = false
        };

        _versionOption = new Option<string>(
            aliases: new[] { "--version", "-v" },
            description: "Version of the package")
        {
            IsRequired = false
        };

        _configOption = new Option<string>(
            aliases: new[] { "--config", "-c" },
            description: "Build configuration (Debug/Release)")
        {
            IsRequired = false
        };

        _optimizationOption = new Option<string>(
            aliases: new[] { "--optimization", "--opt" },
            description: "Optimization level (none, basic, full)")
        {
            IsRequired = false
        };

        // Add options to commands
        microfrontendCommand.AddOption(_dirOption);
        microfrontendCommand.AddOption(_outputOption);
        microfrontendCommand.AddOption(_versionOption);
        microfrontendCommand.AddOption(_configOption);
        microfrontendCommand.AddOption(_optimizationOption);

        appshellCommand.AddOption(_dirOption);
        appshellCommand.AddOption(_outputOption);
        appshellCommand.AddOption(_versionOption);
        appshellCommand.AddOption(_configOption);
        appshellCommand.AddOption(_optimizationOption);

        // Set handlers
        microfrontendCommand.SetHandler(PackageMicrofrontendAsync);
        appshellCommand.SetHandler(PackageAppShellAsync);

        // Add subcommands
        AddCommand(microfrontendCommand);
        AddCommand(appshellCommand);
    }

    private async Task PackageMicrofrontendAsync(InvocationContext context)
    {
        var serviceProvider = context.BindingContext.GetService<IServiceProvider>();
        var packageService = serviceProvider.GetService<IPackageService>();

        var dir = context.ParseResult.GetValueForOption(_dirOption);
        var output = context.ParseResult.GetValueForOption(_outputOption);
        var version = context.ParseResult.GetValueForOption(_versionOption);
        var config = context.ParseResult.GetValueForOption(_configOption);
        var optimization = context.ParseResult.GetValueForOption(_optimizationOption);

        if (string.IsNullOrEmpty(dir))
        {
            dir = AnsiConsole.Ask<string>("Enter the directory containing the microfrontend:");
        }

        if (string.IsNullOrEmpty(output))
        {
            output = Path.Combine(Directory.GetCurrentDirectory(), "packages");
        }

        var options = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(version))
        {
            options["version"] = version;
        }

        if (!string.IsNullOrEmpty(config))
        {
            options["configuration"] = config;
        }

        if (!string.IsNullOrEmpty(optimization))
        {
            options["optimization"] = optimization;
        }

        try
        {
            await AnsiConsole.Status()
                .StartAsync("Packaging microfrontend...", async ctx =>
                {
                    var packagePath = await packageService.PackageMicrofrontendAsync(dir, output, options);
                    ctx.Status($"Package created at: {packagePath}");
                });

            AnsiConsole.MarkupLine("[green]Microfrontend packaged successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            AnsiConsole.MarkupLine("[red]Failed to package microfrontend.[/]");
        }
    }

    private async Task PackageAppShellAsync(InvocationContext context)
    {
        var serviceProvider = context.BindingContext.GetService<IServiceProvider>();
        var packageService = serviceProvider.GetService<IPackageService>();

        var dir = context.ParseResult.GetValueForOption(_dirOption);
        var output = context.ParseResult.GetValueForOption(_outputOption);
        var version = context.ParseResult.GetValueForOption(_versionOption);
        var config = context.ParseResult.GetValueForOption(_configOption);
        var optimization = context.ParseResult.GetValueForOption(_optimizationOption);

        if (string.IsNullOrEmpty(dir))
        {
            dir = AnsiConsole.Ask<string>("Enter the directory containing the app shell:");
        }

        if (string.IsNullOrEmpty(output))
        {
            output = Path.Combine(Directory.GetCurrentDirectory(), "packages");
        }

        var options = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(version))
        {
            options["version"] = version;
        }

        if (!string.IsNullOrEmpty(config))
        {
            options["configuration"] = config;
        }

        if (!string.IsNullOrEmpty(optimization))
        {
            options["optimization"] = optimization;
        }

        try
        {
            await AnsiConsole.Status()
                .StartAsync("Packaging app shell...", async ctx =>
                {
                    var packagePath = await packageService.PackageAppShellAsync(dir, output, options);
                    ctx.Status($"Package created at: {packagePath}");
                });

            AnsiConsole.MarkupLine("[green]App shell packaged successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            AnsiConsole.MarkupLine("[red]Failed to package app shell.[/]");
        }
    }
} 