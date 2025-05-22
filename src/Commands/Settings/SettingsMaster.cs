namespace cli.slndoc.Commands.Settings;

using McMaster.Extensions.CommandLineUtils;

[Command("settings")]
[Subcommand(
    typeof(SettingsShow),
    typeof(SettingsSwitchTo),
    typeof(SettingsSetMaster),
    typeof(SettingsDelete)
)]
internal class SettingsMaster
{
    private void OnExecute(CommandLineApplication app, IConsole console)
    {
        app.ShowHelp();
    }
}