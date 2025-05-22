namespace cli.slndoc.Extensions;
using System.Text.RegularExpressions;

internal static class StringExtensions
{
    public static string RegexReplace(this string @this, string pattern, string replacement, RegexOptions options = RegexOptions.Multiline)
    {
        return Regex.Replace(@this, pattern, replacement, options);
    }
    public static string RegexReplace(this string @this, string pattern, MatchEvaluator matchEvaluator)
    {
        return Regex.Replace(@this, pattern, matchEvaluator);
    }

    private static Regex _genericRegex = new(@"(\w+)<(\w+)>", RegexOptions.Compiled);
    /// <summary>
    /// Replaces "<" and ">" by their html equivalent
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static string GetCleanedMermaidString(this string @this)
        => _genericRegex.Replace(@this, x => $"{x.Groups[1].Value}Of{x.Groups[2].Value}");
        //=> _genericRegex.Replace(@this, x => $"{x.Groups[1].Value}Of{x.Groups[2].Value}[\"{x.Groups[1].Value}&lt;{x.Groups[2].Value}&gt;\"]");

    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

}
