namespace cli.slndoc.Helpers;
using System.Linq;

internal static class TypeHelper
{
    // Common enumerable type names
    private static readonly string[] _enumerableTypes = [
        "IEnumerable",
        "ICollection",
        "IList",
        "List",
        "HashSet",
        "Dictionary",
        "IDictionary",
        "ISet",
        "Collection",
        "ObservableCollection",
        "Array"
    ];

    public static string GetEnumerableElementType(string typeName)
    {
        // Handle generic enumerable types like IEnumerable<T>, List<T>, etc.
        if (typeName.Contains('<') && typeName.Contains('>'))
        {
            var genericStart = typeName.IndexOf('<');
            var genericEnd = typeName.LastIndexOf('>');

            if (genericStart > 0 && genericEnd > genericStart)
            {
                var baseTypeName = typeName.Substring(0, genericStart);
                var genericArguments = typeName.Substring(genericStart + 1, genericEnd - genericStart - 1);

                // Check if it's an enumerable type
                if (IsEnumerableType(baseTypeName))
                {
                    // For simple cases, return the first generic argument
                    // Handle nested generics by finding the first top-level argument
                    return GetFirstGenericArgument(genericArguments);
                }
            }
        }

        // Handle array types like T[]
        if (typeName.EndsWith("[]"))
        {
            return typeName.Substring(0, typeName.Length - 2);
        }

        return null; // Not an enumerable type
    }

    private static string GetFirstGenericArgument(string genericArguments)
    {
        int depth = 0;
        int start = 0;

        for (int i = 0; i < genericArguments.Length; i++)
        {
            char c = genericArguments[i];

            if (c == '<')
                depth++;
            else if (c == '>')
                depth--;
            else if (c == ',' && depth == 0)
            {
                return genericArguments.Substring(start, i - start).Trim();
            }
        }

        return genericArguments.Trim(); // Single argument
    }

    private static bool IsEnumerableType(string baseTypeName)
    {
        return _enumerableTypes.Any(et => baseTypeName.Equals(et) || baseTypeName.EndsWith("." + et));
    }
}
