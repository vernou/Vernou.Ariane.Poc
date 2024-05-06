using NuGet.Versioning;

namespace Vernou.Ariane.Tools.Models;

public class PackageReference
{
    public required PackageDependency Dependency { get; init; }
    public required VersionRange Version { get; init; }
}
