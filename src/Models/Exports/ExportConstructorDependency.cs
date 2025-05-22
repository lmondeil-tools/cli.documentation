namespace cli.slndoc.Models.Exports;
using System.Collections.Generic;

public class ExportConstructorDependency
{
    public string Name { get; set; }
    public IEnumerable<ExportCode> Implementations { get; set; }
}
