using cli.slndoc.Models.Extracted;
using cli.slndoc.Models.Settings;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Text.RegularExpressions;

namespace cli.slndoc.Services.Roslyn;
internal class RoslynExtractionService : IRoslynExtractionService
{
    //public static Func<ExtractedClass, bool> ApiControllersFilter = (x) =>
    //{
    //    try
    //    {
    //        var result = x.Modifier == ExtractedModifier.Public
    //        &&
    //        x.BaseTypes.Contains("ControllerBase");
    //        return result;
    //    }
    //    catch (Exception e)
    //    {
    //        throw;
    //    }
    //};
    private readonly ServicesDependenciesSettings _settings;
    private readonly ILogger<RoslynExtractionService> _logger;


    public RoslynExtractionService(IOptions<ServicesDependenciesSettings> settings, ILogger<RoslynExtractionService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<(IEnumerable<ExtractedClass> AllClasses, IEnumerable<ExtractedInterface> AllInterfaces)> GetAllClassesAndInterfaces(string solutionPath)
    {
        using var workspace = MSBuildWorkspace.Create();

        // Loading solution in Roslyn engine
        _logger.LogInformation("Loading solution {Path} ...", solutionPath);
        var solution = await workspace.OpenSolutionAsync(solutionPath);
        _logger.LogInformation(" OK");

        // Get all Class declarations from roslyn
        _logger.LogInformation("Loading classes...");
        var projects = solution.Projects.Where(p => !_settings.ProjectsExclusionsRegex.Any(r => Regex.IsMatch(p.Name, r)));
        var allClasses = (await Task.WhenAll(projects
                .SelectMany(p => p.Documents)
                .Select(async (x) => await x.GetExtractedClassesAsync(_settings.ExportExclusionsRegex))))
                .SelectMany(x => x)
                .ToArray();
        _logger.LogInformation(" {Count} classes", allClasses.Count());

        // Get all Interface declarations from roslyn
        _logger.LogInformation("Loading interfaces...");
        var allInterfaces =
            (await Task.WhenAll(solution.Projects
                .SelectMany(p => p.Documents)
                .Select(async (x) => await x.GetExtractedInterfacesAsync())))
                .SelectMany(x => x)
                .ToArray();
        _logger.LogInformation("{Count} interfaces", allInterfaces.Count());

        return (allClasses, allInterfaces);
    }

    public async Task<IEnumerable<ExtractedAttribute>> GetAllAttributesAsync(string solutionPath, params string[] attributesTypesNames)
    {
        if (attributesTypesNames == null || attributesTypesNames.Length == 0)
            throw new ArgumentException("No attributes to scan defined", nameof(attributesTypesNames));

        using var workspace = MSBuildWorkspace.Create();
        // Loading solution in Roslyn engine
        _logger.LogInformation("Loading solution {Path} ...", solutionPath);
        var solution = await workspace.OpenSolutionAsync(solutionPath);
        _logger.LogInformation(" OK");
        // Get all Class declarations from roslyn
        _logger.LogInformation("Loading attributes...");
        var allAttributes = (await Task.WhenAll(solution.Projects
                .SelectMany(p => p.Documents)
                .Select(async (x) => await x.GetExtractedAttributesAsync(attributesTypesNames))))
                .SelectMany(x => x)
                .ToArray();
        _logger.LogInformation("{Count} attributes", allAttributes.Count());
        return allAttributes;
    }

    internal IEnumerable<ExtractedClass> GetImplementations(ExtractedCode @code, IEnumerable<ExtractedClass> allClasses)
    {
        var result = allClasses.Where(x => @code.BaseTypes.Contains(x.Name)).ToArray();
        if (result.Length == 0 && @code is ExtractedClass @class)
            return [@class];
        return result;
    }
}
