using ConsoleReflection.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConsoleReflection.Models;
public class ClassDeclarationSyntaxAndSemanticModel
{
    public ClassDeclarationSyntax ClassDeclarationSyntax { get; set; }
    public SemanticModel SemanticModel { get; set; }

    private INamedTypeSymbol? _namedTypeSymbol;
    public INamedTypeSymbol? NamedTypeSymbol => _namedTypeSymbol ??= SemanticModel.GetDeclaredSymbol(ClassDeclarationSyntax) as INamedTypeSymbol;

    public ClassDependencyTree ClassDependencyTree { get; set; }

    public bool IsControllerSemanticModel()
    {
        if (!this.IsPublicClass())
            return false;

        return (this.NamedTypeSymbol?.BaseType?.ToString() ?? "") == "Microsoft.AspNetCore.Mvc.ControllerBase";
    }

    public bool IsPublicClass() => this.ClassDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));

    public bool ImplementsInterface(TypeSyntax interfaceSyntax, IEnumerable<InterfaceDeclarationSyntaxAndSemanticModel> allInterfaces)
    {
        var matchingInterfaces = allInterfaces.Where(i => i.InterfaceDeclarationSyntax.Identifier.Text == interfaceSyntax.ToString());
        var thisInterfacesFullNames = this.NamedTypeSymbol.Interfaces.Select(x => x.ToString());
        var matchingInterfacesFullNames = matchingInterfaces.Select(x => x.GetInterfaceFullName());

        var matching = thisInterfacesFullNames.Intersect(matchingInterfacesFullNames);
        if (matching.Any())
            return true;
        return false;
    }

    public string GetClassFullName()
    {
        string namespaceName = ((NamespaceDeclarationSyntax)this.ClassDeclarationSyntax.Parent).Name.ToString();
        return $"{namespaceName}.{this.GetName()}";
    }
    public string GetName() => this.ClassDeclarationSyntax.Identifier.Text;

    public ClassDependencyTree? GetClassDependencyTree(IEnumerable<ClassDeclarationSyntaxAndSemanticModel> allClassAndModels, IEnumerable<InterfaceDeclarationSyntaxAndSemanticModel> allInterfaces)
    {
        ClassDependencyTree result = new();

        var constructor = this.ClassDeclarationSyntax.DescendantNodes().OfType<ConstructorDeclarationSyntax>().FirstOrDefault(c => c.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)));
        if (constructor is null)
        {
            result.ClassName = this.ClassDeclarationSyntax.Identifier.Text;
            return result;
        }

        var constructorParameters = constructor.ParameterList.Parameters;
        foreach (var parameter in constructorParameters)
        {
            ConsoleExt.WriteLine($"\t\t{this.GetName()} --> {parameter.Type.ToString()}", ConsoleColor.Cyan);
        }

        result.ClassName = this.GetName();
        result.InterfaceImplementationTrees = constructorParameters
                .Select(p => new InterfaceImplementationTree
                {
                    InterfaceName = p.Type.ToString(),
                    Implementations = allClassAndModels.Where(x => x.ImplementsInterface(p.Type, allInterfaces))
                });

        foreach (var item in result.InterfaceImplementationTrees)
        {
            foreach (var implementation in item.Implementations)
            {
                ConsoleExt.WriteLine($"\t\t\t{item.InterfaceName} --> {implementation.GetName()}", ConsoleColor.Gray);
            }
        }


        foreach (var item in result.InterfaceImplementationTrees)
        {
            foreach (var implementation in item.Implementations)
            {
                implementation.ClassDependencyTree = implementation.GetClassDependencyTree(allClassAndModels, allInterfaces);
            }
        }

        return result;
    }
}
