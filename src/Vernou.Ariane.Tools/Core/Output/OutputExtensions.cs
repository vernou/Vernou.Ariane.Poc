namespace Vernou.Ariane.Tools.Core.Output;

internal static class OutputExtensions
{
    public static void Write(this IOutput output, string text)
    {
        output.Write(text, ConsoleHelpers.DefaultConsoleForegroundColor);
    }

    public static void WriteLine(this IOutput output)
    {
        output.Write(Environment.NewLine);
    }

    public static void WriteLine(this IOutput output, string text)
    {
        output.WriteLine(text, ConsoleHelpers.DefaultConsoleForegroundColor);
    }

    public static void WriteLine(this IOutput output, string text, ConsoleColor color)
    {
        output.Write(text, color);
    }
}
