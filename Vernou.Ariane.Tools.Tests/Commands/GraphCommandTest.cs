using Shouldly;
using Vernou.Ariane.Tools.Tests.Core;

namespace Vernou.Ariane.Tools.Tests.Commands;

public sealed class GraphCommandTest
{
    [Fact]
    public async Task ConsoleWithoutDependency()
    {
        // Arrange

        var output = new FakeOutput();
        var command = new Tools.Commands.GraphCommand("""AssetsFiles\ConsoleWithoutDependency\project.assets.json""", output);

        // Act

        await command.RunAsync();

        // Assert

        output.Lines.Count().ShouldBe(2);
        output.Lines.ElementAt(0).ShouldBe(new FakeOutput.Line("ConsoleWithoutDependency@1.0.0"));
        output.Lines.ElementAt(1).ShouldBe(new FakeOutput.Line());
    }

    [Fact]
    public async Task ConsoleWithLibraryWithEFCore()
    {
        // Arrange

        var output = new FakeOutput();
        var command = new Tools.Commands.GraphCommand("""AssetsFiles\ConsoleWithLibraryWithEFCore\project.assets.json""", output);

        // Act

        await command.RunAsync();

        // Assert

        output.Lines.Count().ShouldBe(24);

        output.Lines.ElementAt(00).ShouldBe(new FakeOutput.Line("ConsoleWithLibraryWithEFCore@1.0.0"));
        output.Lines.ElementAt(01).ShouldBe(new FakeOutput.Line("\tLibraryWithEFCore@1.0.0"));
        output.Lines.ElementAt(02).ShouldBe(new FakeOutput.Line("\t\t[8.0.4, ) -> Microsoft.EntityFrameworkCore@8.0.4"));
        output.Lines.ElementAt(03).ShouldBe(new FakeOutput.Line("\t\t\t[8.0.4, ) -> Microsoft.EntityFrameworkCore.Abstractions@8.0.4"));

        output.Lines.ElementAt(04).ShouldBe(new FakeOutput.Line("\t\t\t[8.0.4, ) -> Microsoft.EntityFrameworkCore.Analyzers@8.0.4"));
        output.Lines.ElementAt(05).ShouldBe(new FakeOutput.Line("\t\t\t[8.0.0, ) -> Microsoft.Extensions.Caching.Memory@8.0.0"));
        output.Lines.ElementAt(06).ShouldBe(new FakeOutput.Line("\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Caching.Abstractions@8.0.0"));
        output.Lines.ElementAt(07).ShouldBe(new FakeOutput.Line("\t\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Primitives@8.0.0"));
        output.Lines.ElementAt(08).ShouldBe(new FakeOutput.Line("\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.DependencyInjection.Abstractions@8.0.0"));
        output.Lines.ElementAt(09).ShouldBe(new FakeOutput.Line("\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Logging.Abstractions@8.0.0"));
        output.Lines.ElementAt(10).ShouldBe(new FakeOutput.Line("\t\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.DependencyInjection.Abstractions@8.0.0"));
        output.Lines.ElementAt(11).ShouldBe(new FakeOutput.Line("\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Options@8.0.0"));
        output.Lines.ElementAt(12).ShouldBe(new FakeOutput.Line("\t\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.DependencyInjection.Abstractions@8.0.0"));
        output.Lines.ElementAt(13).ShouldBe(new FakeOutput.Line("\t\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Primitives@8.0.0"));
        output.Lines.ElementAt(14).ShouldBe(new FakeOutput.Line("\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Primitives@8.0.0"));
        output.Lines.ElementAt(15).ShouldBe(new FakeOutput.Line("\t\t\t[8.0.0, ) -> Microsoft.Extensions.Logging@8.0.0"));
        output.Lines.ElementAt(16).ShouldBe(new FakeOutput.Line("\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.DependencyInjection@8.0.0"));
        output.Lines.ElementAt(17).ShouldBe(new FakeOutput.Line("\t\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.DependencyInjection.Abstractions@8.0.0"));
        output.Lines.ElementAt(18).ShouldBe(new FakeOutput.Line("\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Logging.Abstractions@8.0.0"));
        output.Lines.ElementAt(19).ShouldBe(new FakeOutput.Line("\t\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.DependencyInjection.Abstractions@8.0.0"));
        output.Lines.ElementAt(20).ShouldBe(new FakeOutput.Line("\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Options@8.0.0"));
        output.Lines.ElementAt(21).ShouldBe(new FakeOutput.Line("\t\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.DependencyInjection.Abstractions@8.0.0"));
        output.Lines.ElementAt(22).ShouldBe(new FakeOutput.Line("\t\t\t\t\t[8.0.0, ) -> Microsoft.Extensions.Primitives@8.0.0"));
        output.Lines.ElementAt(23).ShouldBe(new FakeOutput.Line());

    }
}
