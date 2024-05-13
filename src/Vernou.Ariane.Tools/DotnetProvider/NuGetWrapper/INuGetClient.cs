using NuGet.Versioning;

namespace Vernou.Ariane.Tools.DotnetProvider.NuGetWrapper;

public interface INuGetClient
{
    Task<PackageVersionInfo> GetPackageVersionInfo(string packageId, SemanticVersion version);
}

public class PackageVersionInfo
{
    public required bool HasVulnerability { get; init; }
    public required bool IsDeprecated { get; init; }
}
