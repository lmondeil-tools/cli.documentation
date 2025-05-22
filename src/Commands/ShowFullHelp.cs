namespace cli.slndoc.Commands;

using cli.slndoc.Extensions;
using McMaster.Extensions.CommandLineUtils;
using System.Text;

[Command]
internal class ShowFullHelp
{
    [Option("-o|--output-markdown", Description = "Output markdown file")]
    public string OutputFile { get; set; }

    private void OnExecute(CommandLineApplication app, IConsole console)
    {
        if (string.IsNullOrEmpty(OutputFile))
        {
            app.Parent.ShowFullHelp(console);
            return;
        }
        StringBuilder sb = new StringBuilder();
        app.Parent.GetFullMarkdownHelp(sb);
        File.WriteAllText(OutputFile, sb.ToString());
        console.WriteLine($"Full help written to {OutputFile}");
    }
}
