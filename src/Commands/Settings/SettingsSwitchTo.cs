namespace cli.slndoc.Commands.Settings;

using cli.slndoc.Services;
using McMaster.Extensions.CommandLineUtils;

[Command("switchto")]
internal class SettingsSwitchTo
{
    [Argument(0)]
    public string Environment { get; set; }

    private async Task OnExecute(CommandLineApplication app, IConsole console)
    {
        await SettingsService.SwitchSettingsAsync(Environment);
    }
}