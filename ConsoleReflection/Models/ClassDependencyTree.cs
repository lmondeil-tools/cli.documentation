namespace ConsoleReflection.Models;
public class ClassDependencyTree
{
    public string ClassName { get; set; }
    public IEnumerable<InterfaceImplementationTree> InterfaceImplementationTrees { get; set; }
}
