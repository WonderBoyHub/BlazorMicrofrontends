using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;

namespace BlazorMicrofrontends.CLI.Services.Implementations;

public class TemplateService : ITemplateService
{
    public Task<IEnumerable<string>> GetAppShellTemplatesAsync()
    {
        return Task.FromResult<IEnumerable<string>>(new List<string>
        {
            "EmptyServer",
            "EmptyWebAssembly",
            "EmptyMaui",
            "SampleServer",
            "SampleWebAssembly",
            "SampleMaui"
        });
    }

    public Task<IEnumerable<string>> GetMicrofrontendTemplatesAsync()
    {
        return Task.FromResult<IEnumerable<string>>(new List<string>
        {
            "Blazor",
            "BlazorMaui",
            "React",
            "Python",
            "Custom"
        });
    }

    public async Task<bool> CreateFromTemplateAsync(string templateName, string outputDirectory, string projectName, Dictionary<string, string> options)
    {
        // Ensure output directory exists
        Directory.CreateDirectory(outputDirectory);
        
        // Map our template names to actual template IDs
        var templateId = MapTemplateNameToId(templateName);
        
        AnsiConsole.WriteLine($"Creating project from template {templateId}...");
        
        // Build the dotnet new command arguments
        string args = $"new {templateId} --name {projectName} --output \"{outputDirectory}\"";
        
        // Add any additional options as parameters
        foreach (var option in options)
        {
            if (!string.IsNullOrEmpty(option.Value))
            {
                args += $" --{option.Key} \"{option.Value}\"";
            }
        }
        
        AnsiConsole.WriteLine($"Running: dotnet {args}");
        
        // Execute the command
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = new Process { StartInfo = processStartInfo };
        process.Start();
        
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();
        
        if (process.ExitCode != 0)
        {
            AnsiConsole.WriteException(new Exception($"Failed to create project from template. Error: {error}"));
            return false;
        }
        
        // Check if the creation was successful
        bool success = Directory.Exists(Path.Combine(outputDirectory, projectName)) || 
                     Directory.GetFiles(outputDirectory).Any() ||
                     Directory.GetDirectories(outputDirectory).Any();
        
        if (success)
        {
            AnsiConsole.WriteLine($"Successfully created project {projectName} in {outputDirectory}");
        }
        else
        {
            AnsiConsole.WriteLine($"Failed to create project {projectName}");
        }
        
        return success;
    }
    
    private string MapTemplateNameToId(string templateName)
    {
        return templateName switch
        {
            "EmptyServer" => "blazormfe-server-empty",
            "EmptyWebAssembly" => "blazormfe-wasm-empty",
            "EmptyMaui" => "blazormfe-maui-empty",
            "SampleServer" => "blazormfe-server-sample",
            "SampleWebAssembly" => "blazormfe-wasm-sample",
            "SampleMaui" => "blazormfe-maui-sample",
            "Blazor" => "blazormfe-blazor",
            "BlazorMaui" => "blazormfe-maui",
            "React" => "blazormfe-react",
            "Python" => "blazormfe-python",
            "Custom" => "blazormfe-custom",
            _ => throw new ArgumentException($"Unknown template name: {templateName}")
        };
    }
} 