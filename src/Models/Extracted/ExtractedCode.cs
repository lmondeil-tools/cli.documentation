namespace cli.slndoc.Models.Extracted;

using System.Collections.Generic;

public class ExtractedCode
{
    public string Name { get; set; }
    public string Namespace { get; set; }
    public ExtractedModifier Modifier { get; set; }
    public IEnumerable<ExtractedAttribute> Attributes { get; set; }
    public IEnumerable<string> BaseTypes { get; set; }
}
