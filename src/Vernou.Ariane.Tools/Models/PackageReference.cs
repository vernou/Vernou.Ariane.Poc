using NuGet.Versioning;

namespace Vernou.Ariane.Tools.Models;

internal class PackageReference
{
    public required PackageDependency Dependency { get; init; }
    public required VersionRange Version { get; init; }
}
