namespace ConsoleApp.Dynamic;

using cli.slndoc.Models.Extracted;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;

public class RuntimeCompiler
{
    public static Func<ExtractedClass, bool> GetFilter(string lambdaCode, params string[] additionalUsings)
    {
        // Build the dynamic source code without using string.Format on a template containing unescaped braces.
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using System.Linq.Expressions;");
        sb.AppendLine("using cli.slndoc.Models.Extracted;");
        sb.AppendLine("using System.Text.RegularExpressions;");
        foreach (var u in additionalUsings)
        {
            if (!string.IsNullOrWhiteSpace(u))
                sb.AppendLine(u);
        }

        sb.AppendLine();
        sb.AppendLine("public class ExtractedClassFilter");
        sb.AppendLine("{");
        sb.Append("    public Expression<Func<ExtractedClass, bool>> Filter => ");
        sb.Append(lambdaCode);
        sb.AppendLine(";");
        sb.AppendLine("}");

        var sourceCode = sb.ToString();

        var instance = CompileSource(sourceCode, "ExtractedClassFilter");
        var expression = (Expression<Func<ExtractedClass, bool>>)((dynamic)instance).Filter;
        return expression.Compile();
    }

    private static object CompileSource(string sourceCode, string className)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var references = new[] {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ExtractedClass).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Text.RegularExpressions.Regex).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Text.Json").Location)
        };

        var compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (var diagnostic in result.Diagnostics)
                exceptions.Add(new Exception(diagnostic.ToString()));
            throw new AggregateException("Compilation failed.", exceptions);
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        var type = assembly.GetType(className);
        return Activator.CreateInstance(type);
    }
}