namespace Vernou.Ariane.Tools.Core.Output;

internal static class OutputExtensions
{
    public static void Write(this IOutput output, string text)
    {
        output.Write(text, ConsoleHelpers.DefaultConsoleForegroundColor);
    }
}
