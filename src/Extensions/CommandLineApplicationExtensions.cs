namespace cli.slndoc.Extensions;

using McMaster.Extensions.CommandLineUtils;

using System.Diagnostics.CodeAnalysis;
using System.Text;

internal static class CommandLineApplicationExtensions
{
    public static void ShowFullHelp(this CommandLineApplication app, IConsole console)
    {
        console?.WriteLine("-----------------------------------------------------------");
        console?.WriteLine($"\t{app.Name}");
        console?.WriteLine("-----------------------------------------------------------");
        app.ShowHelp();
        foreach (var cmd in app.Commands)
        {
            cmd.ShowFullHelp(console!);
        }
    }

    public static void ShowFullMarkdownHelp(this CommandLineApplication app, IConsole console)
    {
        StringBuilder sb = new StringBuilder();
        app.GetFullMarkdownHelp(sb);
        console.WriteLine(sb.ToString());
    }

    public static void GetFullMarkdownHelp(this CommandLineApplication app, StringBuilder sb, string parentCommand = "", bool ignoreMasterCommands = true)
    {
        string commandFullName = $"{parentCommand} {app.Name}".Trim();
        string helpText = app.GetHelpText();

        // Only append command help if it is not a master command or if master commands are to be written
        if (!helpText.Contains("[command] [options]") || !ignoreMasterCommands)
        {
            helpText = helpText.ToMarkdown(commandFullName);
            sb.AppendLine(helpText);
        }

        // Write subcommands
        foreach (var cmd in app.Commands)
        {
            cmd.GetFullMarkdownHelp(sb, commandFullName);
        }
    }
}
