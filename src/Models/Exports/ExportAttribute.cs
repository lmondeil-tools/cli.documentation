namespace cli.slndoc.Models.Exports;
using System.Collections.Generic;

public class ExportAttribute
{
    //private string _id;
    //public string Id => _id ??= $"{Name}#{Guid.NewGuid()}";
    private string _key;
    public string Key => _key ?? $"{Name}#{string.Join("&", Properties.Select(x => $"{x.Key.Trim()}:{x.Value.Trim()}"))}";
    public string Name { get; set; }
    public Dictionary<string, string> Properties { get; set; }
}
