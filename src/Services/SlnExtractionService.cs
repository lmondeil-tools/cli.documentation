using cli.slndoc.Models.Exports;
using cli.slndoc.Models.Extracted;
using cli.slndoc.Models.Settings;
using cli.slndoc.Services.IO;
using cli.slndoc.Services.Mappings;
using cli.slndoc.Services.Roslyn;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cli.slndoc.Services;
internal class SlnExtractionService : ISlnExtractionService
{
    private readonly IOptions<ServicesDependenciesSettings> _servicesDependenciesSettings;
    private readonly IRoslynExtractionService _roslynExtractionService;
    private readonly IIOService _ioService;
    private readonly IMappingService _mappingService;
    private readonly ILogger<SlnExtractionService> _logger;

    public SlnExtractionService(IOptions<ServicesDependenciesSettings> servicesDependenciesSettings, IRoslynExtractionService roslynExtractionService, IIOService ioService, IMappingService mappingService, ILogger<SlnExtractionService> logger)
    {
        _servicesDependenciesSettings = servicesDependenciesSettings;
        _roslynExtractionService = roslynExtractionService;
        _ioService = ioService;
        _mappingService = mappingService;
        _logger = logger;
    }

    public async Task<ExtractedWrapper> ExtractExternalDependenciesAsync(string solutionPath, string serviceName)
    {
        var externalDependenciesAttributes = await _roslynExtractionService.GetAllAttributesAsync(solutionPath, _servicesDependenciesSettings.Value.AttributesToScan.ToArray());
        return new ExtractedWrapper { ServiceName = serviceName, ExternalDependenciesActtributes = externalDependenciesAttributes };
    }

    public async Task<ExtractedWrapper> ExtractApiControllersDependenciesAsync(string solutionPath, string serviceName)
    {
        _logger.LogInformation("Extracting classes and interfaces...");
        var (allClasses, allInterfaces) = await _roslynExtractionService.GetAllClassesAndInterfaces(solutionPath);
        _logger.LogInformation("Extraction done");

        _logger.LogInformation("Filtering documentation attributes...");
        foreach (var @class in allClasses)
        {
            @class.Attributes = @class.Attributes.Where(x => _servicesDependenciesSettings.Value.AttributesToScan.Contains(x.Name));
        }
        foreach (var @interface in allInterfaces)
        {
            @interface.Attributes = @interface.Attributes.Where(x => _servicesDependenciesSettings.Value.AttributesToScan.Contains(x.Name));
        }
        _logger.LogInformation("Documentation attributes filtered");

        _logger.LogInformation("Filtering API controllers...");
        var rootClasses = allClasses.Where(RoslynExtractionService.ApiControllersFilter).ToArray();
        _logger.LogInformation("Found {Count} API controllers", rootClasses.Count());


        return new ExtractedWrapper { ServiceName = serviceName, RootClasses = rootClasses, AllClasses = allClasses, AllInterfaces = allInterfaces };
    }

    public Task GenerateJsonAsync(ExportWrapper exportWrapper, string exportFolder, bool hierarchical = false)
        => _ioService.GenerateJsonAsync(exportWrapper, exportFolder, hierarchical);

    public Task GenerateMermaidAsync(ExportWrapper exportWrapper, string exportFolder)
        => _ioService.GenerateMermaidAsync(_mappingService.Flatten(exportWrapper), exportFolder);

    public Task GenerateYamlAsync(ExportWrapper exportWrapper, string exportFolder, bool hierarchical = false)
        => _ioService.GenerateYamlAsync(exportWrapper, exportFolder, hierarchical);

}
