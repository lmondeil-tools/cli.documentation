namespace cli.slndoc;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class LocalConstants
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = CreateJsonSerializer(false);
    public static readonly JsonSerializerOptions JsonSerializerOptionsIndented = CreateJsonSerializer(true);

    private static JsonSerializerOptions CreateJsonSerializer(bool indented)
    {
        var options = new JsonSerializerOptions { WriteIndented = indented, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
