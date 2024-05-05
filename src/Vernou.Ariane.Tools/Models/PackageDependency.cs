using NuGet.Versioning;

namespace Vernou.Ariane.Tools.Models;

internal class PackageDependency
{
    public required string Name { get; init; }
    public required SemanticVersion ResolvedVersion { get; init; }
    public bool HasVulnerability { get; init; }
    public bool IsDeprecated { get; init; }
    public IEnumerable<PackageReference> PackageReferences => _packageReferences;
    private List<PackageReference> _packageReferences = [];

    public void AddPackageReference(PackageDependency dependency, VersionRange version)
    {
        _packageReferences.Add(new PackageReference {
            Dependency = dependency,
            Version = version
        });
    }

    public override string ToString()
    {
        return $"{Name}@{ResolvedVersion}";
    }
}
