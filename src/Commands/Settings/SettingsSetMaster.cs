namespace cli.slndoc.Commands.Settings;

using McMaster.Extensions.CommandLineUtils;

[Command("set")]
[Subcommand(
    typeof(SettingsSetExportExclusionsRegex),
    typeof(SettingsSetAttributesToScan),
    typeof(SettingsSetRootClassesFilter)
    )]
internal class SettingsSetMaster
{
    private void OnExecute(CommandLineApplication app, IConsole console)
    {
        app.ShowHelp();
    }
}