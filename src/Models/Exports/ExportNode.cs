namespace cli.slndoc.Models.Exports;

using cli.slndoc.Extensions;

using System.Collections.Generic;

public class ExportNode
{
    protected string _key;
    public string Key => _key ?? Type switch
    {
        ExportNodeType.ExternalDependency => $"{Name}#{string.Join("&", Properties?.Select(x => $"{x.Key.Trim()}:{x.Value.Trim()}") ?? Enumerable.Empty<string>())}".Replace(" ", "").GetCleanedMermaidString(),
        _ => Name.GetCleanedMermaidString()
    };
    public string Name { get; set; }

    public string Type { get; set; } = ExportNodeType.Unknown;
    public IDictionary<string, string> Properties { get; set; }
}
