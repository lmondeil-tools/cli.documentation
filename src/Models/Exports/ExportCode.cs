namespace cli.slndoc.Models.Exports;

using cli.slndoc.Models.Extracted;

using System.Text.Json.Serialization;

[JsonDerivedType(typeof(ExportClass), typeDiscriminator: "ExportClass")]
[JsonDerivedType(typeof(ExportInterface), typeDiscriminator: "ExportInterface")]
public class ExportCode
{
    public string Name { get; set; }
    public string Namespace { get; set; }
    public ExtractedModifier Modifier { get; set; }
    public IEnumerable<ExportAttribute> Attributes { get; set; }
    public IEnumerable<string> BaseTypes { get; set; }
}
