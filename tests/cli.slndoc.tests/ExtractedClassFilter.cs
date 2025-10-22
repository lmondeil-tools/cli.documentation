using cli.slndoc.Models.Extracted;

using System.Text.RegularExpressions;

public class ExtractedClassFilter
{
    public Func<ExtractedClass, bool> filter = @class => @class.BaseTypes.Any(t => Regex.IsMatch(t, @"IMyModel\<(\w+)\>"));
}
