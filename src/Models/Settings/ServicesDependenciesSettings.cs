namespace cli.slndoc.Models.Settings;

public class ServicesDependenciesSettings
{
    public IEnumerable<string> AttributesToScan { get; set; }
    public IEnumerable<string> ExportExclusionsRegex { get; set; }
}