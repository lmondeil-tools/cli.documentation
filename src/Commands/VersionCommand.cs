namespace cli.slndoc.Commands;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.Logging;

[Command("version")]
internal class VersionCommand
{
    public void OnExecute(ILogger<VersionCommand> logger)
    {
        var version = typeof(VersionCommand).Assembly.GetName().Version.ToString();
        logger.LogInformation(version);
    }
}
