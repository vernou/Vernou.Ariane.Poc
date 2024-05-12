namespace Vernou.Ariane.Tools.Core.Output;

internal class ConsoleOutput : IOutput
{
    void IOutput.Write(string text, ConsoleColor color)
    {
        ConsoleHelpers.SetConsoleForegroundColor(color);
        Console.Write(text);
        ConsoleHelpers.ResetConsoleForegroundColor();
    }
}
