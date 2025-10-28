using cli.slndoc.Extensions;
using cli.slndoc.Models;
using cli.slndoc.Models.Exports;
using cli.slndoc.Services.Mappings;

using Microsoft.Extensions.Logging;

using Scriban;

using System.Text.Json;

namespace cli.slndoc.Services.IO;
internal class IOService : IIOService
{
    private readonly IMappingService _mappingService;
    private readonly ILogger<IOService> _logger;
    private static readonly string[] _replacableDependencyTypes =
    [
            ExportNodeType.ExternalDependency,
            ExportNodeType.PackageDependency
    ];

    public IOService(IMappingService mappingService, ILogger<IOService> logger)
    {
        _mappingService = mappingService;
        _logger = logger;

    }
    public async Task GenerateJsonAsync(ExportWrapper exportWrapper, string exportFilePath, bool hierarchical = true)
    {
        var exportFolder = Path.GetDirectoryName(exportFilePath);
        if (!Directory.Exists(exportFolder))
            Directory.CreateDirectory(exportFolder);

        var json = hierarchical
            ? JsonSerializer.Serialize(exportWrapper, LocalConstants.JsonSerializerOptionsIndented)
            : JsonSerializer.Serialize(_mappingService.Flatten(exportWrapper), LocalConstants.JsonSerializerOptionsIndented);

        await File.WriteAllTextAsync(exportFilePath, json);
        _logger.LogInformation($"Json file saved at {exportFilePath}");
    }

    public async Task GenerateYamlAsync(ExportWrapper exportWrapper, string exportFilePath, bool hierarchical = true)
    {
        var exportFolder = Path.GetDirectoryName(exportFilePath);
        if (!Directory.Exists(exportFolder))
            Directory.CreateDirectory(exportFolder);

        var yaml = hierarchical
            ? new YamlDotNet.Serialization.Serializer().Serialize(exportWrapper)
            : new YamlDotNet.Serialization.Serializer().Serialize(_mappingService.Flatten(exportWrapper));

        await File.WriteAllTextAsync(exportFilePath, yaml);
        _logger.LogInformation($"yaml file saved at {exportFilePath}");
    }

    /// <summary>
    /// <see cref="https://github.com/scriban/scriban"/>
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="exportFilePath"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task GenerateMermaidAsync(ExportGraph graph, string exportFilePath)
    {
        var exportFolder = Path.GetDirectoryName(exportFilePath);
        if (!Directory.Exists(exportFolder))
            Directory.CreateDirectory(exportFolder);

        var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Templates", "graph2mermaid.withicons.scriban");
        var template = Template.Parse(File.ReadAllText(templateFilePath));
        if(template.HasErrors)
        {
            _logger.LogError("Error parsing template: {Errors}", string.Join(", ", template.Messages.Select(m => m.ToString())));
            throw new InvalidOperationException("Error parsing template for mermaid generation.");
        }
        var mermaidString = await template.RenderAsync(graph);

        await File.WriteAllTextAsync(exportFilePath, mermaidString);
        _logger.LogInformation($"mermaid file saved at {exportFilePath}");
    }

    public ExportWrapper OpenExportWrapper(string exportFilePath)
    {
        try
        {
            return JsonSerializer.Deserialize<ExportWrapper>(File.ReadAllText(exportFilePath), LocalConstants.JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening file {File}", exportFilePath);
            throw;
        }
    }
    public ExportGraph OpenExportGraph(string exportFilePath)
    {
        try
        {
            return JsonSerializer.Deserialize<ExportGraph>(File.ReadAllText(exportFilePath), LocalConstants.JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening file {File}", exportFilePath);
            throw;
        }
    }

    public ExportGraph Merge(IEnumerable<ExportGraph> graphs)
    {
        // Merge all flatinfos
        var result = graphs.Aggregate(new ExportGraph(), (acc, x) =>
        {
            acc.Nodes.AddRange(x.Nodes);
            acc.Edges.AddRange(x.Edges);
            return acc;
        });

        // replace service external dependencies with the service name
        //
        // Find ApiDependency nodes
        // Find corresponding service nodes
        // if the service node exists, replace the ApiDependency link with the service name
        var apiDependencies = result.Nodes
            .Where(x => _replacableDependencyTypes.Contains(x.Type) && (x.Properties?.ContainsKey("ServiceName") ?? false))
            .Select(x => new {Node = x, ServiceName = x.Properties["ServiceName"] })
            .ToArray();
        var services = result.Nodes.Where(x => x.Type == ExportNodeType.Service).ToArray();
        var replaceableApiDependencies = apiDependencies
            .Join(services, a => a.ServiceName, s => s.Name, (a, s) => new { a.Node, ServiceName = s.Name, ServiceKey = s.Key })
            .ToArray();
        var linksToReplace = result.Edges
            .Join(replaceableApiDependencies, l => l.To, a => a.Node.Key, (l, a) => new { Replace = l, By = new ExportEdge { Name = a.ServiceName, From = l.From, To = a.ServiceKey } })
            .ToArray();

        foreach (var item in linksToReplace)
        {
            // Replace the ApiDependency link with the service name
            result.Edges.Remove(item.Replace);
            result.Edges.Add(item.By);
        }

        // remove unused ApiDependencies
        var dependencies = apiDependencies.GroupBy(x => x.Node.Key);
        var links = result.Edges.Where(x => _replacableDependencyTypes.Contains(x.Name))
            .GroupBy(x => x.To);
        var toDeletes = dependencies.GroupJoin(links,
            d => d.Key,
            l => l.Key,
            (d, l) => new { ApiDependency = d, LinkGroup = l })
            .Where(x => !x.LinkGroup.Any())
            .SelectMany(x => x.ApiDependency.Select(g => g.Node))
            .ToArray();

        foreach (var apiDep in toDeletes)
        {
                // Remove the ApiDependency from the result
                result.Nodes.Remove(apiDep);
        }

        // remove duplicates
        var uniqueNodes = result.Nodes
            .GroupBy(x => x.Key)
            .Select(g => g.First())
            .ToList();
        result.Nodes = uniqueNodes;
        var uniqueEdges = result.Edges
            .GroupBy(x => x.Key)
            .Select(g => g.First())
            .ToList();
        result.Edges = uniqueEdges;

        return result;
    }
}
