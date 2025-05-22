namespace cli.slndoc.Services.Mappings;

using cli.slndoc.Helpers;
using cli.slndoc.Models.Exports;
using cli.slndoc.Models.Extracted;

using Microsoft.Build.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Collections.Generic;

public class MappingService : IMappingService
{
    public ExportWrapper MapWrapper(ExtractedWrapper extractedWrapper)
    {
        // Create the envelope
        var result = new ExportWrapper
        {
            ServiceName = extractedWrapper.ServiceName,
            ExternalDependenciesAttributes = extractedWrapper.ExternalDependenciesActtributes?.Select(x => MapAttribute(x)) ?? [],
            RootClasses = MapClasses(extractedWrapper.RootClasses, extractedWrapper.AllClasses, extractedWrapper.AllInterfaces)
        };

        return result;
    }

    public ExportAttribute MapAttribute(ExtractedAttribute attribute)
    {
        try
        {
            return new ExportAttribute
            {
                Name = attribute.Name.Trim(),
                Properties = attribute.Properties?.ToDictionary(x => x.Key.Trim(), x => x.Value.Trim())
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public IEnumerable<ExportClass> MapClasses(IEnumerable<ExtractedClass> extractedClasses, IEnumerable<ExtractedClass> allClasses, IEnumerable<ExtractedInterface> allInterfaces)
    {
        try
        {
            if (extractedClasses is null)
                return [];
            var result = extractedClasses.Select(x => MapClass(x, allClasses, allInterfaces)).ToArray();
            return result;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public ExportClass MapClass(ExtractedClass extractedClass, IEnumerable<ExtractedClass> allClasses, IEnumerable<ExtractedInterface> allInterfaces)
    {
        try
        {
            // Create the envelope
            var result = new ExportClass
            {
                Name = extractedClass.Name.Trim(),
                Namespace = extractedClass.Namespace.Trim(),
                Modifier = extractedClass.Modifier,
                BaseTypes = extractedClass.BaseTypes.Select(x => x.Trim()).ToArray(),
                Attributes = extractedClass.Attributes.Select(x => MapAttribute(x)).ToArray(),
                ConstructorParametersTypesNames = extractedClass.ConstructorParametersTypesNames.ToArray(),
            };

            // Provision dependencies
            result.ConstructorDependencies = extractedClass.ConstructorParametersTypesNames
                .Select(typeName => MapConstructorDependency(typeName, allClasses, allInterfaces))
                .ToArray();

            return result;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public ExportConstructorDependency MapConstructorDependency(string typeName, IEnumerable<ExtractedClass> allClasses, IEnumerable<ExtractedInterface> allInterfaces)
    {
        try
        {
            //var implementations = allClasses.Where(x => x.Name.Equals(typeName));
            var (classImplementations, interfaceImplementations) = ResolveImplementations(typeName, allClasses, allInterfaces);
            var unknownImplementations = new ExportCode { Name = typeName, Attributes = [] };
            var result = new ExportConstructorDependency
            {
                Name = typeName,
                Implementations = classImplementations.Any()
                    ? MapClasses(classImplementations, allClasses, allInterfaces)
                    : interfaceImplementations.Any()
                        ? MapInterfaces(interfaceImplementations)
                        : [unknownImplementations]
            };
            return result;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private static (ExtractedClass[] ClassImplementations, ExtractedInterface[] InterfaceImplementations) ResolveImplementations(string typeName, IEnumerable<ExtractedClass> allClasses, IEnumerable<ExtractedInterface> allInterfaces)
    {
        var classImplementations = allClasses.Where(c => c.BaseTypes.Any(t => t.Equals(typeName))).ToArray();
        var interfaceImplementations = allInterfaces.Where(i => i.Name.Equals(typeName)).ToArray();
        if (classImplementations.Count() + interfaceImplementations.Count() == 0)
        {
            // Check if it's a collection type
            var collectionItemTypeName = TypeHelper.GetEnumerableElementType(typeName);
            if (!string.IsNullOrEmpty(collectionItemTypeName))
            {
                (classImplementations, interfaceImplementations) = ResolveImplementations(collectionItemTypeName, allClasses, allInterfaces);
            }
        }
        return (classImplementations, interfaceImplementations);
    }

    private IEnumerable<ExportInterface> MapInterfaces(IEnumerable<ExtractedInterface> @interfaces)
    {
        try
        {
            return @interfaces.Select(MapInterface).ToArray();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private ExportInterface MapInterface(ExtractedInterface @interface)
    {
        try
        {
            return new ExportInterface
            {
                Name = @interface.Name.Trim(),
                Namespace = @interface.Namespace.Trim(),
                Modifier = @interface.Modifier,
                BaseTypes = @interface.BaseTypes?.Select(x => x.Trim()).ToArray(),
                Attributes = @interface.Attributes?.Select(x => MapAttribute(x)).ToArray()
            };
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public ExportGraph Flatten(ExportCode exportCode, bool iterateOnChildren = true)
    {
        ExportGraph result = new();

        // Current class
        var root = new ExportNode
        {
            Name = exportCode.Name,
            Properties = new Dictionary<string, string> {
                { nameof(ExportCode.Namespace), exportCode.Namespace },
                { nameof(ExportCode.Modifier), exportCode.Modifier.ToString() }
            },
            Type = !string.IsNullOrWhiteSpace(exportCode.Namespace) 
                ? ExportNodeType.InternalDependency 
                : ExportNodeType.InternalDependencyUnresolved
        };
        result.Nodes.Add(root);

        if (!iterateOnChildren)
            return result;

        // External dependencies
        //---------------------------------------------------------------------------
        var externalDependenciesNodes = exportCode.Attributes.Select(x => new ExportNode { Name = x.Name, Properties = x.Properties, Type = ExportNodeType.ExternalDependency }).ToArray();

        // Add nodes
        result.Nodes.AddRange(externalDependenciesNodes);

        // Create links between root and external dependencies
        result.Edges.AddRange(externalDependenciesNodes.Select(dep => new ExportEdge { Name = dep.Name, From = root.Key, To = dep.Key, Type = ExportEdgeType.ExternalDependency }));

        // Internal dependencies
        //---------------------------------------------------------------------------
        List<ExportClass> internalDependencies = [];
        IEnumerable<ExportEdge> links = [];
        if (exportCode is ExportClass exportClass)
        {
            // Get dependencies graphs
            var internalDependenciesGraphs = exportClass.ConstructorDependencies
                .SelectMany(x => x.Implementations)
                .Select(x => Flatten(x));

            // Add nodes and links
            result.Nodes.AddRange(internalDependenciesGraphs.SelectMany(x => x.Nodes));
            result.Edges.AddRange(internalDependenciesGraphs.SelectMany(x => x.Edges));

            // Create links between root and internal dependencies
            result.Edges.AddRange(internalDependenciesGraphs
                .Where(g => g.Root is not null)
                .Select(g => new ExportEdge { Name = g.Root.Name, From = root.Key, To = g.Root.Key }));
        }


        return result;
    }

    public ExportGraph Flatten(ExportWrapper exportWrapper)
    {
        var result = new ExportGraph();

        // Root
        var root = new ExportNode
        {
            Name = exportWrapper.ServiceName,
            Type = ExportNodeType.Service
        };
        result.Nodes.Add(root);

        // Internal dependencies
        var internalDependenciesGraphs = (exportWrapper.RootClasses?
            .Select(x => Flatten(x)) ?? Enumerable.Empty<ExportGraph>());
        result.Nodes.AddRange(internalDependenciesGraphs.SelectMany(x => x.Nodes));
        result.Edges.AddRange(internalDependenciesGraphs.SelectMany(x => x.Edges));

        // Create links between root and internal dependencies
        result.Edges.AddRange(internalDependenciesGraphs
            .Where(g => g.Root is not null)
            .Select(g => new ExportEdge { Name = g.Root.Name, From = root.Key, To = g.Root.Key }));

        // External dependencies
        var externalDependenciesNodes = (exportWrapper.ExternalDependenciesAttributes?
            .Select(x => new ExportNode
            {
                Name = x.Name,
                Properties = x.Properties,
                Type = ExportNodeType.ExternalDependency
            }) ?? Enumerable.Empty<ExportNode>())
            .DistinctBy(x => x.Key);
        result.Nodes.AddRange(externalDependenciesNodes);


        // create links between root and external dependencies
        result.Edges.AddRange(externalDependenciesNodes.Select(dep => new ExportEdge { Name = dep.Name, From = root.Key, To = dep.Key, Type = ExportEdgeType.ExternalDependency }));

        // Clean up the nodes
        result.Nodes = result.Nodes
            .DistinctBy(x => x.Key)
            .ToList();

        // Clean up the edges
        result.Edges = result.Edges
            .DistinctBy(x => x.Key)
            .ToList();

        return result;
    }

}