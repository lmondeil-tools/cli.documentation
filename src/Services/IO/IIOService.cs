using cli.slndoc.Models.Exports;

namespace cli.slndoc.Services.IO;
public interface IIOService
{
    Task GenerateJsonAsync(ExportWrapper exportWrapper, string exportFilePath, bool hierarchical = true);
    Task GenerateMermaidAsync(ExportGraph graph, string exportFilePath);
    Task GenerateYamlAsync(ExportWrapper exportWrapper, string exportFilePath, bool hierarchical = true);
    ExportWrapper OpenExportWrapper(string exportFilePath);
    ExportGraph Merge(IEnumerable<ExportGraph> graphs);
    ExportGraph OpenExportGraph(string exportFilePath);
}