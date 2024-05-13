using NuGet.ProjectModel;

namespace Vernou.Ariane.Tools.DotnetProvider;
public sealed class AssetsProjectResolver : IProjectResolver
{
    Models.Project IProjectResolver.Resolve(string projectPath)
    {
        if(!File.Exists(projectPath))
        {
            throw new InvalidOperationException($"The assets file `{projectPath}` doesn't exist.");
        }

        var lockFileFormat = new LockFileFormat();
        LockFile assetsFile = lockFileFormat.Read(projectPath);

        // Assets file validation
        if(assetsFile.PackageSpec == null ||
            assetsFile.Targets == null ||
            assetsFile.Targets.Count == 0)
        {
            throw new InvalidOperationException($"Unable to read the assets file `{projectPath}`. Please make sure the file has the write format.");
        }

        var resolver = new ProjectResolver(assetsFile);
        return resolver.Resolve();
    }
}
