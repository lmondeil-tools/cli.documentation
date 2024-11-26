using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConsoleReflection.Models;
public class InterfaceDeclarationSyntaxAndSemanticModel
{
    public InterfaceDeclarationSyntax InterfaceDeclarationSyntax { get; set; }
    public SemanticModel SemanticModel { get; set; }

    private INamedTypeSymbol? _namedTypeSymbol;
    public INamedTypeSymbol? NamedTypeSymbol => _namedTypeSymbol ??= SemanticModel.GetDeclaredSymbol(InterfaceDeclarationSyntax) as INamedTypeSymbol;


    public bool IsPublic() => this.InterfaceDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));


    public string GetInterfaceFullName()
    {
        string namespaceName = ((NamespaceDeclarationSyntax)this.InterfaceDeclarationSyntax.Parent).Name.ToString();
        string interfaceName = this.InterfaceDeclarationSyntax.Identifier.Text;
        return $"{namespaceName}.{interfaceName}";
    }

}
