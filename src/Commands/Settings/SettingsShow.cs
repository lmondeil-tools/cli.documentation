namespace cli.slndoc.Commands.Settings;

using cli.slndoc.Models.Settings;
using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

[Command("show")]
internal class SettingsShow
{
    private readonly ServicesDependenciesSettings _settings;

    public SettingsShow(IOptions<ServicesDependenciesSettings> settings)
    {
        _settings = settings.Value;
    }

    private void OnExecute(CommandLineApplication app, IConsole console)
    {
        console.WriteLine("Storage folder :" + AppContext.BaseDirectory);

        var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
        console.WriteLine(JsonSerializer.Serialize(_settings, options: serializerOptions));

        console.WriteLine("Other environments : ");
        var appSettingsFiles = Directory.GetFiles(AppContext.BaseDirectory, "appsettings*.json");

        foreach (var file in appSettingsFiles.ToList().Except(new[] { "appsettings.json" }))
        {
            var env = Regex.Match(file, @"appsettings.(?<env>\w+).json").Groups["env"].Value;
            if (!string.IsNullOrWhiteSpace(env))
            {
                console.WriteLine($"\t * {env}");
            }
        }
    }
}