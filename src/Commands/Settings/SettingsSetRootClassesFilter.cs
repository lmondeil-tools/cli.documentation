using cli.slndoc.Models.Settings;
using cli.slndoc.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cli.slndoc.Commands.Settings;

[Command("root-filter")]
internal class SettingsSetRootClassesFilter
{
    private readonly ServicesDependenciesSettings _settings;

    public SettingsSetRootClassesFilter(IOptions<ServicesDependenciesSettings> settings)
    {
        this._settings = settings.Value;
    }

    [Argument(0, Description = "The lambda expression of type func<ExtractClass, bool> used to filter root classes\nExample: <command> root-filter \"@class => @class.BaseTypes.Any(t => Regex.IsMatch(t, @\"IMyInterface\\<(\\w+)\\>\"))\"")]
    public string Filter{ get; set; }

    [Option("-e|--environment", Description = "Environment to set the settings for")]
    public string? Environment { get; set; }

    private async Task OnExecute2(CommandLineApplication app, IConsole console, ILogger<SettingsSetAttributesToScan> logger)
    {
        try
        {
            var savedFile = await SettingsService.SetValueAsync(this._settings, s => s.RootClassesFilterAsString, Filter, Environment);
            logger.LogInformation($"Attributes to scan saved to {savedFile}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while setting attributes to scan");
        }
    }
}