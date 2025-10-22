namespace cli.slndoc.Models.Settings;

using cli.slndoc.Models.Extracted;

using ConsoleApp.Dynamic;

using System.Text.Json.Serialization;

public class ServicesDependenciesSettings
{
    public IEnumerable<string> AttributesToScan { get; set; } = [];
    public IEnumerable<string> ExportExclusionsRegex { get; set; } = [];
    public string? RootClassesFilterAsString { get; set; }

    private Func<ExtractedClass, bool>? _rootClassesFilter;
    [JsonIgnore]
    public Func<ExtractedClass, bool>? RootClassesFilter => _rootClassesFilter ??=
        string.IsNullOrWhiteSpace(RootClassesFilterAsString)
        ? x => x.Modifier == ExtractedModifier.Public && x.BaseTypes.Contains("ControllerBase")
        : RuntimeCompiler.GetFilter(RootClassesFilterAsString);
}