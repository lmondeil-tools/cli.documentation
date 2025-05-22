namespace cli.slndoc.Extensions;

using System.Text;

internal static class HelpTextExtensions
{
    public static string ToMarkdown(this string helpText, string commandFullName)
    {
        // Add " * * " at the end of the command full name if it is a mster command
        if (helpText.Contains("[command] [options]"))
            commandFullName += " * *";

        // Send Usage content to next line
        helpText = helpText
            .RegexReplace(@"^Usage:(.+)", $"Usage:\n $1")       // Send Usage content to next line
            .RegexReplace(@"^(\w+):", $"### $1")                // Make sections as titles
            .RegexReplace(@"[^\|](-\w\|--\w+(-\w+)*)", $"`$1`") // Make option highlighted
            .Replace("[options]", "`[options]`")
            .RegexReplace(@"\<.*\>", "\t");                     // remove options types


        StringBuilder sb = new();
        sb.AppendLine($"## {commandFullName}");
        sb.AppendLine(helpText);

        return sb.ToString();
    }
}
