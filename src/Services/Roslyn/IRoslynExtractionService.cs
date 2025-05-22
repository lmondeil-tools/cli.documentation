namespace cli.slndoc.Services.Roslyn;

using cli.slndoc.Models.Extracted;

using System.Collections.Generic;
using System.Threading.Tasks;

internal interface IRoslynExtractionService
{
    Task<IEnumerable<ExtractedAttribute>> GetAllAttributesAsync(string solutionPath, params string[] attributesTypesNames);
    Task<(IEnumerable<ExtractedClass> AllClasses, IEnumerable<ExtractedInterface> AllInterfaces)> GetAllClassesAndInterfaces(string solutionPath);
}