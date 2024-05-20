using Vernou.Ariane.Tools.Core.Output;

namespace Vernou.Ariane.Tools.Tests.Core;

internal sealed class FakeOutput : IOutput
{
    private readonly List<(string text, ConsoleColor color)> _writed = new();

    public IEnumerable<(string text, ConsoleColor color)> Writed => _writed;

    public void Write(string text, ConsoleColor color)
    {
        _writed.Add((text, color));
    }
}
