namespace cli.slndoc.Services;

using cli.slndoc.Extensions;

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

public class SettingsService
{
    private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  };

    public static async Task<string> ShowAsync<TSettings>(TSettings settings)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Storage folder :" + AppContext.BaseDirectory);

        sb.AppendLine(JsonSerializer.Serialize(settings, options: LocalConstants.JsonSerializerOptionsIndented));

        sb.AppendLine("Other environments : ");
        var appSettingsFiles = Directory.GetFiles(AppContext.BaseDirectory, "appsettings*.json");

        foreach (var file in appSettingsFiles.ToList().Except(new[] { "appsettings.json" }))
        {
            var env = Regex.Match(file, @"appsettings.(?<env>\w+).json").Groups["env"].Value;
            if (!string.IsNullOrWhiteSpace(env))
            {
                sb.AppendLine($"\t * {env}");
            }
        }

        return sb.ToString();
    }

    public static async Task<string> SetValueAsync<TSettings, TValue>(TSettings? settings, Expression<Func<TSettings, TValue>> propertyAccessor, TValue value, string? environment = null)
        where TSettings : new()
    {
        var result = settings ?? await LoadSettingsAsync<TSettings>(environment) ?? new TSettings();
        ((PropertyInfo)((MemberExpression)propertyAccessor.Body).Member).SetMethod.Invoke(settings, new object[] { value });
        return await SaveSettingsAsync(settings, environment);
    }


    public static async Task SwitchSettingsAsync(string environment)
    {
        string sourceFilePath = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environment}.json");
        string targetFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(sourceFilePath))
            throw new FileNotFoundException();
        File.Copy(sourceFilePath, targetFilePath, true);
    }
    public static async Task DeleteAsync(string environment)
    {
        if (string.IsNullOrWhiteSpace(environment))
            throw new ApplicationException("default settings cannot be deleted");
        string filePath = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environment}.json");
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    private static async Task<string> SaveSettingsAsync<TSettings>(TSettings settings, string? environment = null)
        where TSettings : new()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, string.IsNullOrWhiteSpace(environment) ? "appsettings.json" : $"appsettings.{environment}.json");

        var buffer = new JsonObject();
        if (!File.Exists(filePath))
        {
            buffer[typeof(TSettings).Name.ToCamelCase()] = JsonNode.Parse(JsonSerializer.Serialize(settings));
        }
        else
        {
            string fileContent = await File.ReadAllTextAsync(filePath);
            var jsonStream = JsonSerializer.Serialize(settings, options: _serializerOptions);
            buffer = JsonSerializer.Deserialize<JsonObject>(fileContent);
            buffer[typeof(TSettings).Name.ToCamelCase()].ReplaceWith(settings);
        }

        await File.WriteAllTextAsync(
            filePath,
            JsonSerializer.Serialize(buffer, options: _serializerOptions));

        return filePath;
    }

    private static async Task<TSettings> LoadSettingsAsync<TSettings>(string environment) where TSettings : new()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, string.IsNullOrWhiteSpace(environment) ? "appsettings.json" : $"appsettings.{environment}.json");

        if (!File.Exists(filePath))
            return new TSettings();

        string fileContent = await File.ReadAllTextAsync(filePath);
        var buffer = JsonSerializer.Deserialize<JsonDocument>(fileContent);
        var result = buffer.RootElement.GetProperty(typeof(TSettings).Name).Deserialize<TSettings>();

        return result;
    }
}