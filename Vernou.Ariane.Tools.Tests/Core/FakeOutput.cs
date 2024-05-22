using System.Collections.Generic;
using Vernou.Ariane.Tools.Core.Output;

namespace Vernou.Ariane.Tools.Tests.Core;

internal sealed class FakeOutput : IOutput
{
    private readonly List<Line> _lines = [ new Line() ];
    public IEnumerable<Line> Lines => _lines;

    public void Write(string text, ConsoleColor color)
    {
        var lines = text.Split(Environment.NewLine);
        WriteIfNotEmpty(lines.First());
        foreach (var line in lines.Skip(1))
        {
            _lines.Add(new Line());
            WriteIfNotEmpty(line);
        }

        void WriteIfNotEmpty(string text)
        {
            if(text.Length > 0)
            {
                _lines.Last().Write(text, color);
            }
        }
    }

    public sealed class Line
    {
        private readonly List<(string text, ConsoleColor color)> _writed = [];
        public IEnumerable<(string text, ConsoleColor color)> Writed => _writed;

        public Line(string text) :
            this((text, ConsoleColor.Gray))
        { }

        public Line(params (string text, ConsoleColor color)[] writed)
        {
            _writed.AddRange(writed);
        }

        public void Write(string text, ConsoleColor color)
        {
            if(_writed.Count > 0)
            {
                var last = _writed.Last();
                if(last.color == color)
                {
                    _writed.RemoveAt(_writed.Count - 1);
                    _writed.Add((last.text + text, color));
                    return;
                }
            }
            _writed.Add((text, color));
        }

        public override bool Equals(object? obj)
        {
            if(obj is Line line)
            {
                return Equals(line);
            }
            return false;
        }

        public bool Equals(Line other)
        {
            return _writed.Count == other._writed.Count &&
                _writed.Zip(other._writed).All(z => z.First == z.Second); ;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(_writed.Select(w => w.text)).Replace("\t", @"\t");
        }
    }
}
