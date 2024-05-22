using Vernou.Ariane.Tools.Core.Output;
using Vernou.Ariane.Tools.DotnetProvider;

namespace Vernou.Ariane.Tools.Commands;

public sealed class GraphCommand
{
    private readonly string _projectPath;
    private readonly IOutput _output;

    public GraphCommand(string projectPath, IOutput output)
    {
        _projectPath = projectPath;
        _output = output;
    }

    public async Task RunAsync()
    {
        if(!File.Exists(_projectPath))
        {
            throw new InvalidOperationException($"The project file `{_projectPath}` doesn't exist.");
        }

        var fileName = Path.GetFileName(_projectPath);
        IProjectResolver resolver;
        if(fileName == "project.assets.json")
        {
            resolver = new AssetsProjectResolver();
        }
        else
        {
            throw new InvalidOperationException("Unresolvable project type.");
        }

        var project = resolver.Resolve(_projectPath);
        Display(project, 0);
        await Task.CompletedTask;
    }

    private void Display(Models.Project project, int level)
    {
        _output.WriteTabs(level);
        _output.WriteLine($"{project}");
        foreach(var reference in project.ProjectReferences)
        {
            Display(reference, level + 1);
        }
        foreach(var reference in project.PackageReferences)
        {
            Display(reference, level + 1);
        }
    }

    private void Display(Models.PackageReference package, int level)
    {
        _output.WriteTabs(level);
        _output.Write($"{package.Version} -> {package.Dependency}");
        if(package.Dependency.HasVulnerability)
        {
            _output.Write(" (has vulnerability)", ConsoleColor.Red);
        }
        if(package.Dependency.IsDeprecated)
        {
            _output.Write(" (is deprecated)", ConsoleColor.Yellow);
        }
        _output.WriteLine();
        foreach(var reference in package.Dependency.PackageReferences)
        {
            Display(reference, level + 1);
        }
    }
}
