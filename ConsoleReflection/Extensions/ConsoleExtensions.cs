namespace ConsoleReflection.Extensions;
internal static class ConsoleExt
{
    public static void WriteLine(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
