namespace cli.slndoc.Models.RoslynWrap;

public class DocumentationAttribute
{
    private string _id;
    public string Id => _id ??= $"{Name}#{Guid.NewGuid()}";
    public string Name { get; set; }
    public IDictionary<string, string> Properties { get; set; }

    public string GetMermaidModel()
        => $"{Id}[\"{Name}<br/> {string.Join("<br/>", ((Dictionary<string, string>)Properties).Select(x => $"{x.Key}: {x.Value}"))} \"]";
}