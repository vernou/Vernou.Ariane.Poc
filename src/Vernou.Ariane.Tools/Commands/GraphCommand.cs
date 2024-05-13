using NuGet.ProjectModel;
using Vernou.Ariane.Tools.DotnetProvider;

namespace Vernou.Ariane.Tools.Commands;
internal class GraphCommand
{
    private readonly string _projectPath;

    public GraphCommand(string projectPath)
    {
        _projectPath = projectPath;
    }

    public async Task RunAsync()
    {

        //var projectName = """HowFix.Demo""";
        //var assetsPath = """C:\t\HowFix.Demo\HowFix.Demo\obj\project.assets.json""";
        //var assetsPath = """C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""";

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

    static void Display(Models.Project project, int level)
    {
        Align(level);
        Console.WriteLine($"{project}");
        foreach(var reference in project.ProjectReferences)
        {
            Display(reference, level + 1);
        }
        foreach(var reference in project.PackageReferences)
        {
            Display(reference, level + 1);
        }
    }

    static void Display(Models.PackageReference package, int level)
    {
        Align(level);
        Console.Write($"{package.Version} -> {package.Dependency}");
        if(package.Dependency.HasVulnerability)
        {
            var origin = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" (has vulnerability)");
            Console.ForegroundColor = origin;
        }
        if(package.Dependency.IsDeprecated)
        {
            var origin = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" (is deprecated)");
            Console.ForegroundColor = origin;
        }
        Console.WriteLine();
        foreach(var reference in package.Dependency.PackageReferences)
        {
            Display(reference, level + 1);
        }
    }

    static void Align(int level)
    {
        for(var i = 0; i < level; i++)
        {
            Console.Write('\t');
        }
    }
}
