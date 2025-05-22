using cli.slndoc.Models.Settings;
using cli.slndoc.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cli.slndoc.Commands.Settings;

[Command("exclude-classes")]
internal class SettingsSetExportExclusionsRegex
{
    private readonly ServicesDependenciesSettings _settings;

    public SettingsSetExportExclusionsRegex(IOptions<ServicesDependenciesSettings> settings)
    {
        this._settings = settings.Value;
    }

    [Argument(0, Description = "List of the names of the regexes to exclude class names\nExample: <command> exclude-classes ^ILogger<.* ^string$")]
    public string[] ExclusionRegexex { get; set; }

    [Option("-e|--environment", Description = "Environment to set the settings for")]
    public string? Environment { get; set; }

    private async Task OnExecute(CommandLineApplication app, IConsole console, ILogger<SettingsSetExportExclusionsRegex> logger)
    {
        try
        {
            var savedFile = await SettingsService.SetValueAsync(this._settings, s => s.ExportExclusionsRegex, ExclusionRegexex, Environment);
            logger.LogInformation($"Attributes to scan saved to {savedFile}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while setting attributes to scan");
        }
    }
}