using cli.slndoc.Models.Exports;
using cli.slndoc.Models.Extracted;

namespace cli.slndoc.Services;
public interface ISlnExtractionService //: IExportWrapperIOService
{
    Task<ExtractedWrapper> ExtractApiControllersDependenciesAsync(string solutionPath, string serviceName);
    Task<ExtractedWrapper> ExtractExternalDependenciesAsync(string solutionPath, string serviceName);

    Task GenerateJsonAsync(ExportWrapper exportWrapper, string exportFilePath, bool hierarchical = false);
    Task GenerateMermaidAsync(ExportWrapper exportWrapper, string exportFilePath);
    Task GenerateYamlAsync(ExportWrapper exportWrapper, string exportFilePath, bool hierarchical = false);

}