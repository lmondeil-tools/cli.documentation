using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Text.Json.Serialization;

using YamlDotNet.Serialization;

namespace cli.slndoc.Models.RoslynWrap;
public class InterfaceDeclarationSyntaxAndSemanticModel
{
    public InterfaceDeclarationSyntax InterfaceDeclarationSyntax { get; set; }
    public SemanticModel SemanticModel { get; set; }

    private INamedTypeSymbol? _namedTypeSymbol;
    public INamedTypeSymbol? NamedTypeSymbol => _namedTypeSymbol ??= SemanticModel.GetDeclaredSymbol(InterfaceDeclarationSyntax) as INamedTypeSymbol;

    private IEnumerable<AttributeSyntax>? _attributeSyntaxes;
    [YamlIgnore]
    [JsonIgnore]
    public IEnumerable<AttributeSyntax> AttributeSyntaxes => _attributeSyntaxes ??= InterfaceDeclarationSyntax.DescendantNodes()
                .OfType<AttributeSyntax>();

    public IEnumerable<DocumentationAttribute> DocumentationAttributes { get; set; }


    public bool IsPublic() => InterfaceDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));


    public string GetInterfaceFullName()
    {
        string namespaceName = string.Empty;
        if (InterfaceDeclarationSyntax.Parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclarationSyntax)
            namespaceName = fileScopedNamespaceDeclarationSyntax.Name.ToString();
        if (InterfaceDeclarationSyntax.Parent is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
            namespaceName = namespaceDeclarationSyntax.Name.ToString();

        string interfaceName = InterfaceDeclarationSyntax.Identifier.Text;
        return $"{namespaceName}.{interfaceName}";
    }

}
