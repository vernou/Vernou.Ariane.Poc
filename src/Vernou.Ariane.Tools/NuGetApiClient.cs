using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Vernou.Ariane.Tools;

public class NuGetApiClient
{
    private readonly SourceRepository _repository;
    private readonly PackageMetadataResource _packageMetadataResource;
    private readonly SourceCacheContext _sourceCacheContext;

    public NuGetApiClient()
    {
        _repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        _packageMetadataResource = _repository.GetResource<PackageMetadataResource>();
        _sourceCacheContext = new SourceCacheContext();
    }

    public async Task<IPackageSearchMetadata> GetPackageVersionMetadata(string packageId, SemanticVersion version)
    {
        var packageMetadata = await GetPackageMetadata(packageId);
        var packageMedatadaWhere = packageMetadata.Where(m => m.Identity.Version == version).ToList();
        if(packageMedatadaWhere.Count == 0)
        {
            throw new InvalidOperationException($"{packageId}@{version} was not found from NuGet.org.");
        }
        if(packageMedatadaWhere.Count > 1)
        {
            throw new InvalidOperationException($"Many {packageId}@{version} were found from NuGet.org.");
        }
        return packageMedatadaWhere[0];
    }

    public async Task<IEnumerable<IPackageSearchMetadata>> GetPackageMetadata(string packageId)
    {
        var packageMetadata = await _packageMetadataResource.GetMetadataAsync(
            packageId,
            true,
            true,
            _sourceCacheContext,
            NuGet.Common.NullLogger.Instance,
            CancellationToken.None);
        return packageMetadata;
    }
}
