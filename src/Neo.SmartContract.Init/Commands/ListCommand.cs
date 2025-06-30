using System.CommandLine;
using Spectre.Console;
using Neo.SmartContract.Init.Services;

namespace Neo.SmartContract.Init.Commands;

public class ListCommand : Command
{
    public ListCommand() : base("list", "List available project templates")
    {
        this.SetHandler(() =>
        {
            var templateService = new TemplateService();
            var templates = templateService.GetAvailableTemplates();

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Available NEO Smart Contract Templates:[/]");
            AnsiConsole.WriteLine();

            var table = new Table();
            table.AddColumn("[bold]Template ID[/]");
            table.AddColumn("[bold]Description[/]");
            table.AddColumn("[bold]Features[/]");
            table.Border(TableBorder.Rounded);

            foreach (var template in templates)
            {
                table.AddRow(
                    $"[green]{template.Id}[/]",
                    template.Description,
                    string.Join(", ", template.Features)
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[dim]Use [bold]neo-init new <project-name> --template <template-id>[/] to create a project[/]");
        });
    }
}