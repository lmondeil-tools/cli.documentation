namespace cli.slndoc.Commands;

using cli.slndoc.Commands.Exports;
using cli.slndoc.Commands.Extracts;
using cli.slndoc.Commands.Merges;
using cli.slndoc.Commands.Pings;
using cli.slndoc.Commands.Settings;
using cli.slndoc.Extensions;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[Command("slndoc")]
[Subcommand(
    typeof(ShowFullHelp),
    typeof(Extract),
    typeof(Merge),
    typeof(Export),
    typeof(SettingsMaster),
    typeof(Ping)
)]
internal class MainCommand
{
    private IHostEnvironment _env;
    private readonly ILogger _logger;

    public MainCommand(IHostEnvironment env, ILogger<MainCommand> logger)
    {
        _env = env;
        _logger = logger;
    }

    private void OnExecute(CommandLineApplication app, IConsole console)
    {
        console.WriteLine("You must specify a subcommand.");
        console.WriteLine("See documentation below.");
        console.WriteLine("****************************************************.");
        console.WriteLine();
        app.ShowFullHelp(console);
    }
}
