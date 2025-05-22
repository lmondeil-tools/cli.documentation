namespace cli.slndoc.Models.Extracted;

using System.Collections.Generic;

public class ExtractedClass: ExtractedCode
{
    public IEnumerable<string> ConstructorParametersTypesNames { get; set; }
}
