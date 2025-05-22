namespace cli.slndoc.Commands.Exports;
using cli.slndoc.Commands.Extracts;
using cli.slndoc.Services.IO;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.Logging;

[Command("export", Description = "Loads a non hierarchical json file and generates a mermaid file.")]

internal class Export
{
    private readonly IIOService _iOService;

    public Export(IIOService iOService)
    {
        this._iOService = iOService;
    }

    [Option("-i|--input-path", Description = "The path to the sln file")]
    public string InputFilePath { get; set; }

    [Option("-o|--output-path", Description = "The path to the resulting mermaid file.")]
    public string OutputPath { get; set; }

    public async Task OnExecuteAsync(ILogger<Extract> logger, CancellationToken cancellationToken)
    {
        // Validate input file path
        if (string.IsNullOrWhiteSpace(InputFilePath) || !File.Exists(InputFilePath))
        {
            logger.LogError("Invalid input file path: {InputFilePath}", InputFilePath);
            return;
        }
        // Validate output file path
        if (string.IsNullOrWhiteSpace(OutputPath))
        {
            logger.LogError("Output file path is required.");
            return;
        }
        
        var infos = _iOService.OpenExportGraph(InputFilePath);
        _iOService.GenerateMermaidAsync(infos, OutputPath);
    }
}
