using NuGet.ProjectModel;
using Vernou.Ariane.Tools.Core.Output;
using Vernou.Ariane.Tools.DotnetProvider;

namespace Vernou.Ariane.Tools.Commands;
internal class AuditCommand
{
    private readonly string _projectPath;
    private readonly IOutput _output;

    public AuditCommand(string projectPath, IOutput output)
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

    static bool HasProblem(Models.Project project)
    {
        return project.ProjectReferences.Any(HasProblem) ||
            project.PackageReferences.Any(HasProblem);
    }

    static bool HasProblem(Models.PackageReference packageReference)
    {
        return HasProblem(packageReference.Dependency);
    }

    static bool HasProblem(Models.PackageDependency packageReference)
    {
        return packageReference.IsDeprecated ||
            packageReference.HasVulnerability ||
            packageReference.PackageReferences.Any(HasProblem);
    }

    private void Display(Models.Project project, int level)
    {
        _output.WriteTabs(level);
        _output.WriteLine($"{project}");
        foreach(var reference in project.ProjectReferences)
        {
            if(HasProblem(reference))
            {
                Display(reference, level + 1);
            }
        }
        foreach(var reference in project.PackageReferences)
        {
            Display(reference, level + 1);
        }
    }

    private void Display(Models.PackageReference package, int level)
    {
        if(HasProblem(package.Dependency))
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
}
