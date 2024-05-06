using NuGet.ProjectModel;
using Shouldly;

namespace Vernou.Ariane.Tools.Tests;

public class ProjectResolverTest
{
    [Fact]
    public void ResolveSimpleConsoleAppProject()
    {
        // Arrange

        LockFile assetsFile;
        var lockFileFormat = new LockFileFormat();
        assetsFile = lockFileFormat.Read("""AssetsFiles\SimpleConsoleApp.assets.json""");

        // Act

        var resolver = new ProjectResolver(assetsFile);
        var project = resolver.Resolve();

        // Assert

        project.Name.ShouldBe("SimpleConsoleApp");
        project.ResolvedVersion.ShouldBe(new NuGet.Versioning.SemanticVersion(1, 0, 0));
        project.HasVulnerability.ShouldBeFalse();
        project.IsDeprecated.ShouldBeFalse();
        project.ProjectReferences.ShouldBeEmpty();
        project.PackageReferences.ShouldBeEmpty();
    }
}
