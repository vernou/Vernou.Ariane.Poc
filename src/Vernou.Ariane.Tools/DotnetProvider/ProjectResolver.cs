using NuGet.ProjectModel;
using System.Globalization;
using Vernou.Ariane.Tools.DotnetProvider.NuGetWrapper;

namespace Vernou.Ariane.Tools.DotnetProvider;

public sealed class ProjectResolver
{
    private readonly LockFile _assetsFile;
    private readonly Dictionary<string, Models.PackageDependency> _packagesCache = new();
    private readonly Dictionary<string, Models.Project> _projectsCache = new();
    private readonly LockFileTarget _target;
    private readonly LockFileTargetLibrary[] _packageLibraries;
    private readonly LockFileTargetLibrary[] _projectLibraries;
    private readonly INuGetClient _nugetClient = new NuGetClient();
     
    public ProjectResolver(LockFile assetsFile)
    {
        _assetsFile = assetsFile;
        // Filtering the Targets to ignore TargetFramework + RID combination, only keep TargetFramework in requestedTargets.
        // So that only one section will be shown for each TFM.
        _target = _assetsFile.Targets.Where(t => t.RuntimeIdentifier == null).Single();
        _packageLibraries = _target.Libraries.Where(l => l.Type == "package").ToArray();
        _projectLibraries = _target.Libraries.Where(l => l.Type == "project").ToArray();
    }

    public Models.Project Resolve()
    {
        var rootDependencies = _assetsFile.PackageSpec.TargetFrameworks.Single(tfm => tfm.FrameworkName.Equals(_target.TargetFramework)).Dependencies;

        var root = new Models.Project
        {
            Name = _assetsFile.PackageSpec.Name,
            ResolvedVersion = _assetsFile.PackageSpec.Version
        };

        foreach (var projectDependency in rootDependencies)
        {
            var packageDependency = GetPackageDependency(projectDependency.Name);
            root.AddPackageReference(packageDependency, projectDependency.LibraryRange.VersionRange!);
        }

        foreach (var projectLibrary in _projectLibraries)
        {
            if (string.IsNullOrEmpty(projectLibrary.Name))
            {
                throw new InvalidOperationException("Project Reference haven't a name.");
            }
            var projectDependency = GetProject(projectLibrary.Name);
            root.AddProjectReference(projectDependency);
        }

        return root;
    }

    private Models.PackageDependency GetPackageDependency(string name)
    {
        if (_packagesCache.TryGetValue(name, out var dependency))
        {
            return dependency;
        }

        var foundLibraries = _packageLibraries.Where(l => l.Name!.Equals(name)).ToList();
        if (foundLibraries.Count == 0)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "No library with the name `{0}` found in the assets file.", name));
        }

        if (foundLibraries.Count > 1)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Multiple libraries with the same name `{0}` found in the assets file.", name));
        }

        var library = foundLibraries[0];
        var packageVersionInfo = _nugetClient.GetPackageVersionInfo(library.Name!, library.Version!).Result;

        var dep = new Models.PackageDependency
        {
            Name = library.Name!,
            ResolvedVersion = library.Version!,
            HasVulnerability = packageVersionInfo.HasVulnerability,
            IsDeprecated = packageVersionInfo.IsDeprecated
        };
        _packagesCache.Add(name, dep);

        foreach (var d in library.Dependencies)
        {
            var subDep = GetPackageDependency(d.Id);
            dep.AddPackageReference(subDep, d.VersionRange);
        }
        return dep;
    }

    Models.Project GetProject(string name)
    {
        if (_projectsCache.TryGetValue(name, out var projectFromCache))
        {
            return projectFromCache;
        }

        var foundLibraries = _projectLibraries.Where(l => l.Name!.Equals(name));
        if (!foundLibraries.Any())
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "No project with the name `{0}` found in the assets file.", name));
        }

        if (foundLibraries.Count() > 1)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Multiple projects with the same name `{0}` found in the assets file.", name));
        }

        var projectLibrary = _projectLibraries.Single(l => l.Name!.Equals(name));

        var project = new Models.Project
        {
            Name = projectLibrary.Name ?? throw new InvalidOperationException("Library haven't a name."),
            ResolvedVersion = projectLibrary.Version ?? throw new InvalidOperationException("Library haven't a version.")
        };
        _projectsCache.Add(name, project);
        foreach (var projectDependency in projectLibrary.Dependencies)
        {
            if (_projectLibraries.Any(l => l.Name == projectDependency.Id))
            {
                project.AddProjectReference(GetProject(projectDependency.Id));
            }
            else
            {
                project.AddPackageReference(GetPackageDependency(projectDependency.Id), projectDependency.VersionRange);
            }
        }
        return project;
    }
}