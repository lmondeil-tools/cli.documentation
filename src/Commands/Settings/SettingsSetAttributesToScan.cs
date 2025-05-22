using cli.slndoc.Models.Settings;
using cli.slndoc.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cli.slndoc.Commands.Settings;

[Command("attributes-to-scan")]
internal class SettingsSetAttributesToScan
{
    private readonly ServicesDependenciesSettings _settings;

    public SettingsSetAttributesToScan(IOptions<ServicesDependenciesSettings> settings)
    {
        this._settings = settings.Value;
    }

    [Argument(0, Description = "List of the names of the attributes that represent a dependency : Array of string in json format\nExample: <command> attributes-to-scan attr1 attr2 attr3")]
    public string[] Attributes { get; set; }

    [Option("-e|--environment", Description = "Environment to set the settings for")]
    public string? Environment { get; set; }

    private async Task OnExecute(CommandLineApplication app, IConsole console, ILogger<SettingsSetAttributesToScan> logger)
    {
        try
        {
            var savedFile = await SettingsService.SetValueAsync(this._settings, s => s.AttributesToScan, Attributes, Environment);
            logger.LogInformation($"Attributes to scan saved to {savedFile}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while setting attributes to scan");
        }
    }
}