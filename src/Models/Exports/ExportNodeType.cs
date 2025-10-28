    namespace cli.slndoc.Models.Exports;

public static class ExportNodeType
{
    public const string Unknown = "Unknown";
    public const string Service = "Service";
    public const string InternalDependency = "InternalDependency";
    public const string InternalDependencyUnresolved = "InternalDependencyUnresolved";
    public const string ExternalDependency = "ExternalDependency";
    public const string PackageDependency = "PackageDependency";
}