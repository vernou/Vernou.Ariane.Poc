using NuGet.ProjectModel;

namespace Vernou.Ariane.Tools.Commands;
internal class AuditCommand
{
    public async Task RunAsync()
    {
        var projectName = """HowFix.Demo""";
        //var assetsPath = """C:\t\HowFix.Demo\HowFix.Demo\obj\project.assets.json""";
        var assetsPath = """C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""";

        if(!File.Exists(assetsPath))
        {
            throw new InvalidOperationException($"No assets file was found for `{projectName}`. Please run restore before running this command.");
        }

        var lockFileFormat = new LockFileFormat();
        LockFile assetsFile = lockFileFormat.Read(assetsPath);

        // Assets file validation
        if(assetsFile.PackageSpec == null ||
            assetsFile.Targets == null ||
            assetsFile.Targets.Count == 0)
        {
            throw new InvalidOperationException($"Unable to read the assets file `{assetsPath}`. Please make sure the file has the write format.");
        }

        var resolver = new ProjectResolver(assetsFile);
        var project = resolver.Resolve();
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

    static void Display(Models.Project project, int level)
    {
        Align(level);
        Console.WriteLine($"{project}");
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

    static void Display(Models.PackageReference package, int level)
    {
        if(HasProblem(package.Dependency))
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
    }

    static void Align(int level)
    {
        for(var i = 0; i < level; i++)
        {
            Console.Write('\t');
        }
    }
}
