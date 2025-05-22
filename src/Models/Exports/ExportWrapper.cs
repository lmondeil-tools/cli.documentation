namespace cli.slndoc.Models.Exports;

using System.Collections.Generic;

public class ExportWrapper
{
    public string ServiceName { get; set; }
    public IEnumerable<ExportAttribute> ExternalDependenciesAttributes { get; set; }
    public IEnumerable<ExportClass> RootClasses { get; set; }

    public override string ToString()
    {
        return new YamlDotNet.Serialization.Serializer().Serialize(this);
    }
}
