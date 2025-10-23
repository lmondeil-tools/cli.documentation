namespace cli.slndoc.Models.Exports;
public class ExportEdge
{
    protected string _key;
    public string Key => _key ?? $"{From} --> {To}";
    public string Name { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Type { get; set; } = ExportEdgeType.Unknown;
}
