namespace Vernou.Ariane.Tools.Core;

internal static class ConsoleHelpers
{
    private static readonly bool ColorsAreSupported;
    public static readonly ConsoleColor DefaultConsoleForegroundColor;

    static ConsoleHelpers()
    {
        if (!(OperatingSystem.IsBrowser() || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsTvOS()))
        {
            ColorsAreSupported = true;
            DefaultConsoleForegroundColor = Console.ForegroundColor;
        }
    }

    internal static void SetConsoleForegroundColor(ConsoleColor color)
    {
        if (ColorsAreSupported)
        {
            Console.ForegroundColor = color;
        }
    }

    internal static void ResetConsoleForegroundColor()
    {
        if (ColorsAreSupported)
        {
            Console.ResetColor();
        }
    }
}