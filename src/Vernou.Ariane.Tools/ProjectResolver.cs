using NuGet.ProjectModel;
using System.Globalization;
using System.Xml.Linq;

namespace Vernou.Ariane.Tools;

internal class ProjectResolver
{
    private readonly LockFile _assetsFile;
    private readonly Dictionary<string, Models.PackageDependency> _packagesCache = new();
    private readonly Dictionary<string, Models.Project> _projectsCache = new();
    private readonly LockFileTarget _target;
    private readonly LockFileTargetLibrary[] _packageLibraries;
    private readonly LockFileTargetLibrary[] _projectLibraries;
    private readonly NuGetApiClient _nugetApi = new();

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

        var root = new Models.Project {
            Name = _assetsFile.PackageSpec.Name,
            ResolvedVersion = _assetsFile.PackageSpec.Version
        };

        foreach(var projectDependency in rootDependencies)
        {
            var packageDependency = GetPackageDependency(projectDependency.Name);
            root.AddPackageReference(packageDependency, projectDependency.LibraryRange.VersionRange!);
        }

        return root;
    }

    private Models.PackageDependency GetPackageDependency(string name)
    {
        if(_packagesCache.TryGetValue(name, out var dependency))
        {
            return dependency;
        }

        var foundLibraries = _packageLibraries.Where(l => l.Name!.Equals(name)).ToList();
        if(foundLibraries.Count == 0)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "No library with the name `{0}` found in the assets file.", name));
        }

        if(foundLibraries.Count > 1)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Multiple libraries with the same name `{0}` found in the assets file.", name));
        }

        var library = foundLibraries[0];

        var hasVulnerability = false;
        var isDeprecated = false;
        var metadata = _nugetApi.GetPackageVersionMetadata(library.Name!, library.Version!).Result;
        if(metadata.Vulnerabilities?.Any() ?? false)
        {
            hasVulnerability = true;
        }
        if(metadata.GetDeprecationMetadataAsync().Result is not null)
        {
            isDeprecated = true;
        }

        var dep = new Models.PackageDependency {
            Name = library.Name!,
            ResolvedVersion = library.Version!,
            HasVulnerability = hasVulnerability,
            IsDeprecated = isDeprecated
        };
        _packagesCache.Add(name, dep);

        foreach(var d in library.Dependencies)
        {
            var subDep = GetPackageDependency(d.Id);
            dep.AddPackageReference(subDep, d.VersionRange);
        }
        return dep;
    }

    Models.Project GetProject(string name)
    {
        if(_projectsCache.TryGetValue(name, out var projectFromCache))
        {
            return projectFromCache;
        }

        var foundLibraries = _projectLibraries.Where(l => l.Name!.Equals(name));
        if(!foundLibraries.Any())
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "No project with the name `{0}` found in the assets file.", name));
        }

        if(foundLibraries.Count() > 1)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Multiple projects with the same name `{0}` found in the assets file.", name));
        }

        var projectLibrary = _projectLibraries.Single(l => l.Name!.Equals(name));

        var project = new Models.Project {
            Name = projectLibrary.Name,
            ResolvedVersion = projectLibrary.Version
        };
        _projectsCache.Add(name, project);
        foreach(var projectDependency in projectLibrary.Dependencies)
        {
            if(_projectLibraries.Any(l => l.Name == projectDependency.Id))
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