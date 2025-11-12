namespace cli.slndoc.Commands.Settings;

using cli.slndoc.Services;
using McMaster.Extensions.CommandLineUtils;

[Command("save")]
internal class SettingsSave
{
    [Argument(0)]
    public string Environment { get; set; }

    private async Task OnExecute(CommandLineApplication app, IConsole console)
    {
        await SettingsService.SaveAsync(Environment);
    }
}