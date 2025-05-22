namespace cli.slndoc.Commands.Merges;

using cli.slndoc.Models.Exports;
using cli.slndoc.Services.IO;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.Logging;

using System.Text.Json;

[Command("merge", Description = "Merge two or more exported documents into one document.")]
internal class Merge
{
    private readonly IIOService _ioService;

    public Merge(IIOService ioService)
    {
        this._ioService = ioService;
    }

    [Argument(0, Description = "The files to merge.")]
    public IEnumerable<string> FilesPaths { get; set; }

    [Option("-o|--output", Description = "The output file path for the merged document.")]
    public string OutputFilePath { get; set; }

    private void OnExecute(IConsole console, ILogger<Merge> logger)
    {
        // OpenExportWrapper files
        var data = new List<ExportGraph>();
        foreach (var filePath in this.FilesPaths)
        {
            try
            {
                var fileData = JsonSerializer.Deserialize<ExportGraph>(File.ReadAllText(filePath), LocalConstants.JsonSerializerOptions);
                data.Add(fileData);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error opening file {filePath}", filePath);
            }
        }

        // Merge
        var result = this._ioService.Merge(data);

        var json = JsonSerializer.Serialize(result, LocalConstants.JsonSerializerOptions);

        File.WriteAllText(this.OutputFilePath, json);
        logger.LogInformation("Merged {Count} files into {OutputFilePath}", data.Count, this.OutputFilePath);
    }

}
