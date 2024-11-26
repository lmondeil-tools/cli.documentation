using ConsoleReflection.Extensions;
using ConsoleReflection.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

string solutionPath = @"C:\dev\p.fnac\src\back\unit-service\fnaccomnav.unitservice.offer\fnaccomnav.unitservice.offer.sln";
using (var workspace = MSBuildWorkspace.Create())
{
    ConsoleExt.WriteLine("Loading solution...", ConsoleColor.Yellow);
    var solution = await workspace.OpenSolutionAsync(solutionPath);

    ConsoleExt.WriteLine("Loading classes...", ConsoleColor.Yellow);
    var allClasses =
        (await Task.WhenAll(solution.Projects
            .SelectMany(p => p.Documents)
            .Select(async (Document x) => await GetClassDeclarationSyntaxAsync(x))))
            .SelectMany(x => x);

    ConsoleExt.WriteLine("Loading interfaces...", ConsoleColor.Yellow);
    var allInterfaces =
        (await Task.WhenAll(solution.Projects
            .SelectMany(p => p.Documents)
            .Select(async (Document x) => await GetInterfaceDeclarationSyntaxAsync(x))))
            .SelectMany(x => x);


    var publicControllerClasses = await GetPublicControllerDependencyTree(allClasses, allInterfaces);
}

static async Task<IEnumerable<ClassDeclarationSyntaxAndSemanticModel>> GetClassDeclarationSyntaxAsync(Document document)
{
    var syntaxTree = await document.GetSyntaxTreeAsync();
    var root = await syntaxTree.GetRootAsync();
    var semanticModel = await document.GetSemanticModelAsync();
    var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
    return classes.Select(x => new ClassDeclarationSyntaxAndSemanticModel { SemanticModel = semanticModel, ClassDeclarationSyntax = x });
}

static async Task<IEnumerable<InterfaceDeclarationSyntaxAndSemanticModel>> GetInterfaceDeclarationSyntaxAsync(Document document)
{
    var syntaxTree = await document.GetSyntaxTreeAsync();
    var root = await syntaxTree.GetRootAsync();
    var semanticModel = await document.GetSemanticModelAsync();
    var interfaces = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>();
    return interfaces.Select(x => new InterfaceDeclarationSyntaxAndSemanticModel { SemanticModel = semanticModel, InterfaceDeclarationSyntax = x });
}

static async Task<List<string>> GetPublicControllerDependencyTree(IEnumerable<ClassDeclarationSyntaxAndSemanticModel> allClasses, IEnumerable<InterfaceDeclarationSyntaxAndSemanticModel> allInterfaces)
{
    ConsoleExt.WriteLine("Loading controllers...", ConsoleColor.Yellow);
    var publicControllerClasses = new List<string>();

    var controllerClasses = allClasses
        .Where(c => c.IsControllerSemanticModel());

    foreach ( var c in controllerClasses)
        ConsoleExt.WriteLine($"\t{c.GetName()}", ConsoleColor.Gray);

    ConsoleExt.WriteLine("Iterating controllers...", ConsoleColor.Yellow);
    foreach (var classAndModel in controllerClasses)
    {
        Console.WriteLine();
        ConsoleExt.WriteLine($"\t%%{classAndModel.GetName()}%%", ConsoleColor.DarkYellow);
        publicControllerClasses.Add(classAndModel.ClassDeclarationSyntax.ToString());
        var controllerTree = classAndModel.GetClassDependencyTree(allClasses, allInterfaces);
    }

    return publicControllerClasses;
}