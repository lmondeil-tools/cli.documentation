using cli.slndoc.Models.Exports;
using cli.slndoc.Services;
using cli.slndoc.Services.Mappings;

using Docker.DotNet.Models;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.Logging;

namespace cli.slndoc.Commands.Extracts
{
    [Command("extract", Description = "Parses a solution (.sln) starting from api controllers and creates a dependency tree.")]
    public class Extract
    {
        private readonly IMappingService _mappingService;
        private readonly ISlnExtractionService _slnExtractionService;

        public Extract(IMappingService mappingService, ISlnExtractionService extractionService)
        {
            _mappingService = mappingService;
            _slnExtractionService = extractionService;
        }

        [Option("-i|--input-path", Description = "The path to the sln file")]
        public string InputFilePath { get; set; }

        [Option("-o|--output-path", Description = "The path to the result file. The file type is given by one of the following extensions : json, yaml|yml, mmd, svg")]
        public string OutputPath { get; set; }

        [Option("-r|--root-name", Description = "[Required. the name of the root of the graph")]
        public string? RootName { get; set; }

        [Option("-e|--external-only", optionType:CommandOptionType.NoValue, Description = "[Optional] (default : false). Determines if internal classes should be fetched (false) or not (true)")]
        public bool ExternalOnly { get; set; } = false;

        [Option("-h|--hierarchical", Description = "[Optional] (default: false) Determines if the export format is hierarchical or not. This option has no impact on mermaid exrection.")]
        public bool Hierarchical { get; set; } = false;

        public async Task OnExecuteAsync(ILogger<Extract> logger, CancellationToken cancellationToken)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(RootName))
                    RootName = Path.GetFileNameWithoutExtension(InputFilePath);

                // We can extract the graph starting from the API controllers or the only external dependencies
                var extractedWrapper = ExternalOnly
                    ? await this._slnExtractionService.ExtractExternalDependenciesAsync(this.InputFilePath, RootName)
                    : await this._slnExtractionService.ExtractApiControllersDependenciesAsync(this.InputFilePath, RootName);
                ExportWrapper exportWrapper = _mappingService.MapWrapper(extractedWrapper);


                switch (Path.GetExtension(this.OutputPath))
                {
                    case ".yml":
                    case ".yaml":
                        await this._slnExtractionService.GenerateYamlAsync(exportWrapper, OutputPath, Hierarchical);
                        break;
                    case ".mmd":
                        await this._slnExtractionService.GenerateMermaidAsync(exportWrapper, OutputPath);
                        break;
                    case ".svg":
                        string tempPath = Path.ChangeExtension(OutputPath, "mmd");
                        string exportFolder = Path.GetDirectoryName(tempPath);
                        string mmdTempFileName = Path.GetFileName(tempPath);
                        await this._slnExtractionService.GenerateMermaidAsync(exportWrapper, tempPath);
                        try
                        {
                            logger.LogInformation($"Trying to run command : docker run -v {exportFolder}:/data minlag/mermaid-cli -i {mmdTempFileName} --iconPacks @iconify-json/logos @iconify-json/teenyicons @iconify-json/devicon @iconify-json/carbon");
                            List<string> commandParameters = [
                                "-i", mmdTempFileName,
                                "--iconPacks", "@iconify-json/logos", "@iconify-json/teenyicons", "@iconify-json/devicon", "@iconify-json/carbon"];
                            await new ContainerService(logger).RunAsync("minlag/mermaid-cli", "mermaid-cli-tmp", commandParameters, new HostConfig
                            {
                                Binds = [$"{exportFolder}:/data"]
                            });
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning("Run the following command to generate the image:");
                            logger.LogWarning($"docker run --rm -v {exportFolder}:/data minlag/mermaid-cli -i {mmdTempFileName} --iconPacks @iconify-json/logos @iconify-json/teenyicons @iconify-json/devicon @iconify-json/carbon");
                        }
                        break;
                    case ".json":
                    default:
                        await this._slnExtractionService.GenerateJsonAsync(exportWrapper, OutputPath, Hierarchical);
                        break;
                }
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "An error occurred while extracting the documentation");
            }
        }
    }
}
