using Microsoft.Extensions.Logging;

namespace cli.slndoc.Extensions;
internal static class ILoggerExtensions
{
    private static YamlDotNet.Serialization.Serializer _yamlSerializer = new();

    public static void WriteYaml(this ILogger logger, object o)
    {
        try
        {
            logger.LogInformation(_yamlSerializer.Serialize(o));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error when displaying yaml");
        }
        Console.ResetColor();
    }
}
