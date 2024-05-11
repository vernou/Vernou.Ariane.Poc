using NuGet.ProjectModel;
using Shouldly;

namespace Vernou.Ariane.Tools.Tests;

public class ProjectResolverTest
{
    [Fact]
    public void ConsoleWithoutDependency()
    {
        // Arrange

        LockFile assetsFile;
        var lockFileFormat = new LockFileFormat();
        assetsFile = lockFileFormat.Read("""AssetsFiles\ConsoleWithoutDependency.assets.json""");

        // Act

        var resolver = new ProjectResolver(assetsFile);
        var project = resolver.Resolve();

        // Assert

        project.Name.ShouldBe("ConsoleWithoutDependency");
        project.ResolvedVersion.ShouldBe(new NuGet.Versioning.SemanticVersion(1, 0, 0));
        project.HasVulnerability.ShouldBeFalse();
        project.IsDeprecated.ShouldBeFalse();
        project.ProjectReferences.ShouldBeEmpty();
        project.PackageReferences.ShouldBeEmpty();
    }

    [Fact]
    public void ConsoleWithLibraryWithEFCore()
    {
        // Arrange

        LockFile assetsFile;
        var lockFileFormat = new LockFileFormat();
        assetsFile = lockFileFormat.Read("""AssetsFiles\ConsoleWithLibraryWithEFCore.assets.json""");

        // Act

        var resolver = new ProjectResolver(assetsFile);
        var project = resolver.Resolve();

        // Assert

        project.Name.ShouldBe("ConsoleWithLibraryWithEFCore");
        project.ResolvedVersion.ShouldBe(new NuGet.Versioning.SemanticVersion(1, 0, 0));
        project.HasVulnerability.ShouldBeFalse();
        project.IsDeprecated.ShouldBeFalse();
        project.PackageReferences.ShouldBeEmpty();

        var projectReference = project.ProjectReferences.ShouldHaveSingleItem();
        projectReference.Name.ShouldBe("LibraryWithEFCore");
        projectReference.ResolvedVersion.ShouldBe(new NuGet.Versioning.SemanticVersion(1, 0, 0));
        projectReference.HasVulnerability.ShouldBeFalse();
        projectReference.IsDeprecated.ShouldBeFalse();
        projectReference.ProjectReferences.ShouldBeEmpty();

        var packageReference = projectReference.PackageReferences.ShouldHaveSingleItem();
        packageReference.Version.ShouldBe(new NuGet.Versioning.VersionRange(new NuGet.Versioning.NuGetVersion(8, 0, 4)));

        var dependencyPackage = packageReference.Dependency;
        dependencyPackage.Name.ShouldBe("Microsoft.EntityFrameworkCore");
        dependencyPackage.ResolvedVersion.ShouldBe(new NuGet.Versioning.SemanticVersion(8, 0, 4));
        dependencyPackage.HasVulnerability.ShouldBeFalse();
        dependencyPackage.IsDeprecated.ShouldBeFalse();
        dependencyPackage.PackageReferences.Count().ShouldBe(4);
    }
}
