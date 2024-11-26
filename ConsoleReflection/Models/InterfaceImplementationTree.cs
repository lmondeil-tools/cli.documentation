namespace ConsoleReflection.Models;

public class InterfaceImplementationTree
{
    public string InterfaceName { get; set; }
    public IEnumerable<ClassDeclarationSyntaxAndSemanticModel> Implementations { get; set; }
}