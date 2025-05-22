namespace cli.slndoc.Models.Exports;

using System.Collections.Generic;

public abstract class ExportInfo
{
    protected string _key;
    public abstract string Key { get; }
    public string Name { get; set; }
}
