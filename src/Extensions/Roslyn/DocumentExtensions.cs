using cli.slndoc.Models.Extracted;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable IDE0130, S1200
namespace Microsoft.CodeAnalysis;
#pragma warning restore IDE130, S1200
/// <summary>
/// Provides extension methods for the <see cref="Document"/> class to extract class and interface declarations.
/// </summary>
internal static class DocumentExtensions
{
    internal static SyntaxKind[] _orderedModifiers = [SyntaxKind.PublicKeyword, SyntaxKind.InternalKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword];

    /// <summary>
    /// Asynchronously retrieves the syntax root of the specified document.
    /// </summary>
    /// <param name="document">The document to analyze.</param>
    internal static async Task<SyntaxNode> GetSyntaxRoot(this Document document)
    {
        var syntaxTree = await document.GetSyntaxTreeAsync();
        return await syntaxTree!.GetRootAsync();
    }

    internal static async Task<IEnumerable<ExtractedClass>> GetExtractedClassesAsync(this Document document, IEnumerable<string> exclusionRegexes)
    {
        var root = await document.GetSyntaxRoot();
        var classes = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>();

        return classes.Select(x => GetExtractedClass(x, exclusionRegexes));
    }

    internal static ExtractedClass GetExtractedClass(ClassDeclarationSyntax @class, IEnumerable<string> constructorParametersTypeNameExclusionRegexes)
    {
        var name = @class.Identifier.Text;

        // TODO : Store usings

        var @namespace = @class.Parent switch
        {
            NamespaceDeclarationSyntax ns => ns.Name.ToString(),
            FileScopedNamespaceDeclarationSyntax fileScopedNs => fileScopedNs.Name.ToString(),
            _ => string.Empty
        };

        IEnumerable<SyntaxKind> orderedModifiers = [SyntaxKind.PublicKeyword, SyntaxKind.InternalKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword];

        var modifier = GetExtractedModifier(@class.Modifiers);

        var attributes = @class.AttributeLists
            .SelectMany(x => x.Attributes)
            .Select(GetExtractedAttribute)
            .ToArray();

        var baseTypes = @class.BaseList?.Types.ToArray().Select(x => x.ToString()) ?? Enumerable.Empty<string>();

        var constructor = @class
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault(c => c.ParameterList?.Parameters.Any() ?? false);
        var constructorParametersTypesNames = constructor?.ParameterList?.Parameters.Select(p => p.Type.ToString()).ToArray() 
            ?? @class.ParameterList?.Parameters.Select(p => p.Type.ToString()).ToArray()
            ?? Enumerable.Empty<string>();

        // Remove excluded types
        constructorParametersTypesNames = constructorParametersTypeNameExclusionRegexes?.Any() ?? false
            ? constructorParametersTypesNames.Where(tpn => !constructorParametersTypeNameExclusionRegexes.Any(r => System.Text.RegularExpressions.Regex.IsMatch(tpn, r)))
            : constructorParametersTypesNames;

        return new ExtractedClass
        {
            Name = name,
            Namespace = @namespace,
            Modifier = modifier,
            Attributes = attributes,
            ConstructorParametersTypesNames = constructorParametersTypesNames,
            BaseTypes = baseTypes
        };
    }

    internal static ExtractedModifier GetExtractedModifier(SyntaxTokenList modifiers)
    {
        var modifier = _orderedModifiers.Join(
                modifiers.Where(m => (int)m.Kind() is >= 8343 and <= 8346),
                o => o,
                c => c.Kind(),
                (o, c) => o
            ).FirstOrDefault(SyntaxKind.InternalKeyword);

        return modifier switch
        {
            SyntaxKind.PublicKeyword => ExtractedModifier.Public,
            SyntaxKind.InternalKeyword => ExtractedModifier.Internal,
            SyntaxKind.ProtectedKeyword => ExtractedModifier.Protected,
            SyntaxKind.PrivateKeyword => ExtractedModifier.Private,
            _ => ExtractedModifier.None
        };
    }
    internal static async Task<IEnumerable<ExtractedInterface>> GetExtractedInterfacesAsync(this Document document)
    {
        var root = await document.GetSyntaxRoot();
        var classes = root.DescendantNodes()
            .OfType<InterfaceDeclarationSyntax>();

        return classes.Select(GetExtractedInterface);
    }

    internal static ExtractedInterface GetExtractedInterface(InterfaceDeclarationSyntax @interface)
    {
        var name = @interface.Identifier.Text;

        var @namespace = @interface.Parent switch
        {
            NamespaceDeclarationSyntax ns => ns.Name.ToString(),
            FileScopedNamespaceDeclarationSyntax fileScopedNs => fileScopedNs.Name.ToString(),
            _ => string.Empty
        };

        var modifier = GetExtractedModifier(@interface.Modifiers);

        var attributes = @interface.AttributeLists
            .SelectMany(x => x.Attributes)
            .Select(GetExtractedAttribute)
            .ToArray();

        var baseTypes = @interface.BaseList?.Types.ToArray().Select(x => x.ToString());


        return new ExtractedInterface
        {
            Name = name,
            Namespace = @namespace,
            Modifier = modifier,
            Attributes = attributes,
            BaseTypes = baseTypes
        };
    }

    internal static ExtractedAttribute GetExtractedAttribute(AttributeSyntax attributeSyntax)
    {
        var buffer = attributeSyntax?.ArgumentList?.Arguments
            .Select(x => x.ToString().Replace("\"", "").Split("=", StringSplitOptions.RemoveEmptyEntries));

        // TODO : try to get parameters names from attribute class constructor
        var properties = buffer?.Where(arr => arr.Length == 2).Select(arr => (name: arr[0].Trim(), value: arr[1].Trim()));
        var parameters = buffer?.Where(arr => arr.Length == 1).Select(arr => arr[0].Trim());
        if (parameters?.Any() ?? false)
        {
            var namelessProperties = (name: "parameters", value: string.Join(",", parameters));
            if (properties is null)
                properties = [namelessProperties];
            else
                properties = properties.Append(namelessProperties);
        }

        return new ExtractedAttribute
        {
            Name = attributeSyntax.Name.ToString(),
            Properties = properties?.ToDictionary()
        };
    }

    internal static async Task<IEnumerable<ExtractedAttribute>> GetExtractedAttributesAsync(this Document document, params string[] watchedAttributesTypeNames)
    {
        var root = await document.GetSyntaxRoot();
        var semanticModel = await document.GetSemanticModelAsync();
        var attributes = root.DescendantNodes()
            .OfType<AttributeSyntax>()
            .Where(x => watchedAttributesTypeNames.Contains(x.Name.ToString()))
            .Select(GetExtractedAttribute);

        return attributes;
    }
}
