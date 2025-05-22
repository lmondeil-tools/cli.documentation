namespace cli.slndoc.Models.Exports;
using System.Collections.Generic;

public class ExportClass: ExportCode
{
    public IEnumerable<string> ConstructorParametersTypesNames { get; set; }

    public IEnumerable<ExportConstructorDependency> ConstructorDependencies { get; set; }
}
