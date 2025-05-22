namespace cli.slndoc.Models.Extracted;

using System.Collections.Generic;

public class ExtractedWrapper
{
    public string ServiceName { get; set; }
    public IEnumerable<ExtractedAttribute> ExternalDependenciesActtributes { get; set; }
    public IEnumerable<ExtractedClass> RootClasses { get; set; }
    public IEnumerable<ExtractedClass> AllClasses { get; set; } = new List<ExtractedClass>();
    public IEnumerable<ExtractedInterface> AllInterfaces { get; set; } = new List<ExtractedInterface>();
}
