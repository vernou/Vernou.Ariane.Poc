namespace Vernou.Ariane.Tools;
internal interface IOuput
{
    public void Write(string text, ConsoleColor color);
}

internal class ConsoleOutput : IOuput
{
    void IOuput.Write(string text, ConsoleColor color)
    {
        ConsoleHelpers.SetConsoleForegroundColor(color);
        Console.Write(text);
        ConsoleHelpers.ResetConsoleForegroundColor();
    }
}

internal static class OuputExtensions
{
    public static void Write(this IOuput output, string text)
    {
        output.Write(text, ConsoleHelpers.DefaultConsoleForegroundColor);
    }
}

internal static class ConsoleHelpers
{
    private static readonly bool ColorsAreSupported;
    public static readonly ConsoleColor DefaultConsoleForegroundColor;

    static ConsoleHelpers()
    {
        if(!(OperatingSystem.IsBrowser() || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsTvOS()))
        {
            ColorsAreSupported = true;
            DefaultConsoleForegroundColor = Console.ForegroundColor;
        }
    }

    internal static void SetConsoleForegroundColor(ConsoleColor color)
    {
        if(ColorsAreSupported)
        {
            Console.ForegroundColor = color;
        }
    }

    internal static void ResetConsoleForegroundColor()
    {
        if(ColorsAreSupported)
        {
            Console.ResetColor();
        }
    }
}