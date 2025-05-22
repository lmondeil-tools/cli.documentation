namespace cli.slndoc.Models.Exports;
using System.Collections.Generic;

public class ExportGraph
{
    public List<ExportNode> Nodes { get; set; } = new ();
    public List<ExportEdge> Edges { get; set; } = new();
    public ExportNode? Root => this.Nodes.FirstOrDefault();
}
