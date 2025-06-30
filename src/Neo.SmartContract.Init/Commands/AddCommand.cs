using System;
using System.CommandLine;
using System.Threading.Tasks;
using Spectre.Console;
using Neo.SmartContract.Init.Services;

namespace Neo.SmartContract.Init.Commands;

public class AddCommand : Command
{
    public AddCommand() : base("add", "Add components to an existing project")
    {
        var componentArgument = new Argument<string>(
            "component",
            "The component to add (tests, docker, ci, analyzer)");

        var pathOption = new Option<string>(
            new[] { "--path", "-p" },
            getDefaultValue: () => ".",
            "Path to the project");

        AddArgument(componentArgument);
        AddOption(pathOption);

        this.SetHandler(async (string component, string path) =>
        {
            var projectService = new ProjectService();

            await AnsiConsole.Status()
                .StartAsync($"Adding {component} to project...", async ctx =>
                {
                    try
                    {
                        switch (component.ToLower())
                        {
                            case "tests":
                                await projectService.AddTestProject(path);
                                break;
                            case "docker":
                                await projectService.AddDockerSupport(path);
                                break;
                            case "ci":
                                await projectService.AddGitHubActions(path);
                                break;
                            case "analyzer":
                                await projectService.AddSecurityAnalyzer(path);
                                break;
                            default:
                                AnsiConsole.MarkupLine($"[red]Unknown component: {component}[/]");
                                return;
                        }

                        AnsiConsole.MarkupLine($"[green]✓[/] Successfully added {component} to project");
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]✗[/] Failed to add {component}: {ex.Message}");
                    }
                });
        }, componentArgument, pathOption);
    }
}
